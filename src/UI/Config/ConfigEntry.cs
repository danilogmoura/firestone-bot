using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.UI.Config;

internal enum ConfigEntryKind
{
    Unsupported,
    Bool,
    Float,
    Int,
    Text,
    Keybind,
    MultiSelect,
    SingleSelect,

    /// <summary>Ordered list of steps. Text in the .cfg, edited by a purpose-built widget.</summary>
    ComboSteps
}

/// <summary>Valid range of a numeric control. Always comes from a declared range, never from a guess.</summary>
internal readonly struct NumberRange
{
    public NumberRange(float min, float max, bool wholeNumbers)
    {
        Min = min;
        Max = max;
        WholeNumbers = wholeNumbers;
    }

    public float Min { get; }
    public float Max { get; }
    public bool WholeNumbers { get; }
}

/// <summary>
///     A choice option of an entry: the id that goes into the .cfg and the name the UI shows.
///     The split exists so the file keeps storing the format the tasks already understand.
/// </summary>
internal readonly struct ConfigChoice
{
    public ConfigChoice(string id, string label)
    {
        Id = id;
        Label = label;
    }

    public string Id { get; }
    public string Label { get; }
}

/// <summary>
///     Option set declared for an entry, with single-vs-multiple decided at the declaration.
///     <para>
///         Exclusivity cannot come from the type: 'asc'/'desc' and '0,1,2' are both string, and only
///         the declaration knows that one accepts a combination and the other does not.
///     </para>
/// </summary>
internal sealed class ChoiceSet
{
    public ChoiceSet(ConfigChoice[] choices, bool multiple)
    {
        Choices = choices;
        Multiple = multiple;
    }

    public ConfigChoice[] Choices { get; }
    public bool Multiple { get; }
}

/// <summary>
///     Typed view of a MelonPreferences_Entry.
///     Reads and writes through the public <c>Value</c> property of the generic entry, so no private
///     field of any task is touched — the .cfg remains the single source of truth.
/// </summary>
internal sealed class ConfigEntry
{
    private readonly PropertyInfo _valueProperty;

    public ConfigEntry(MelonPreferences_Category category, MelonPreferences_Entry entry, NumberRange range,
        ChoiceSet choiceSet = null, bool comboSteps = false)
    {
        Category = category;
        Entry = entry;
        Range = range;
        Choices = choiceSet?.Choices ?? Array.Empty<ConfigChoice>();

        var declaredType = DeclaredTypeOf(entry);
        ValueType = declaredType;
        Kind = Classify(declaredType);

        // Declared options change the widget, not what goes into the file: an int entry with options
        // is still an Int32 in the .cfg, it only swaps the slider for chips. The registry hands over
        // a ChoiceSet only when the type can hold the choice, so here it is just a matter of deciding
        // between one option and several.
        if (choiceSet != null)
            switch (Kind)
            {
                case ConfigEntryKind.Text when choiceSet.Multiple:
                    Kind = ConfigEntryKind.MultiSelect;
                    break;

                case ConfigEntryKind.Text:
                case ConfigEntryKind.Int:
                case ConfigEntryKind.Float:
                    Kind = ConfigEntryKind.SingleSelect;
                    break;
            }

        // A list of steps is text in the .cfg like any other string; only the widget changes, because a text
        // field accepts the sequence without ever showing what was understood from it.
        if (comboSteps && Kind == ConfigEntryKind.Text) Kind = ConfigEntryKind.ComboSteps;

        _valueProperty = entry.GetType().GetProperty("Value");
        if (_valueProperty == null || !_valueProperty.CanWrite)
        {
            Kind = ConfigEntryKind.Unsupported;
            ConfigRegistry.LogUnwritable(Category, entry);
        }
    }

    public MelonPreferences_Category Category { get; }
    public MelonPreferences_Entry Entry { get; }
    public ConfigEntryKind Kind { get; }
    public NumberRange Range { get; }

    /// <summary>Real type of the value in the .cfg. It is what decides how a choice id is written.</summary>
    public Type ValueType { get; }

    /// <summary>Declared options. Empty on every kind other than MultiSelect or SingleSelect.</summary>
    public ConfigChoice[] Choices { get; }

    public string Key => $"{Category.Identifier}/{Entry.Identifier}";

    public string Label => string.IsNullOrWhiteSpace(Entry.DisplayName) ? Entry.Identifier : Entry.DisplayName;

    /// <summary>Text MelonLoader stores as a comment in the .cfg — the tasks use real "\n" breaks.</summary>
    public string Description => Entry.Description ?? string.Empty;

    public bool ReadBool() => ReadRaw() is bool value && value;

    public float ReadNumber()
    {
        var raw = ReadRaw();
        return raw == null ? 0f : Convert.ToSingle(raw);
    }

    public string ReadText() => ReadRaw() as string ?? string.Empty;

    /// <summary>
    ///     Ids marked in the current value. Ids outside the declared list are dropped — the same rule
    ///     AlchemistTask applies when reading the .cfg, so the panel never shows an option the task
    ///     would ignore.
    /// </summary>
    public List<string> ReadSelection()
    {
        var selected = new List<string>();
        var value = ReadText();
        if (string.IsNullOrWhiteSpace(value)) return selected;

        foreach (var part in value.Split(','))
        {
            var id = part.Trim();
            if (id.Length == 0 || selected.Contains(id)) continue;

            foreach (var choice in Choices)
                if (choice.Id == id)
                {
                    selected.Add(id);
                    break;
                }
        }

        return selected;
    }

    /// <summary>
    ///     Serializes the selection in declaration order, not in click order, so the .cfg comes out
    ///     the same regardless of how the user marked the boxes.
    ///     With nothing marked it writes an empty string: exactly the case AlchemistTask treats as
    ///     "no resources" (its test is IsNullOrWhiteSpace, so empty and null land in the same branch).
    /// </summary>
    public string EncodeSelection(List<string> selected)
    {
        var parts = new List<string>();

        foreach (var choice in Choices)
            if (selected.Contains(choice.Id))
                parts.Add(choice.Id);

        return string.Join(",", parts);
    }

    /// <summary>
    ///     Id of the marked option, or null when the .cfg value matches none of the declared ones.
    ///     Returning null there leaves the row with nothing lit, instead of lighting the wrong option.
    /// </summary>
    public string ReadChoice()
    {
        var raw = ReadRaw();
        if (raw == null) return null;

        var id = Convert.ToString(raw, CultureInfo.InvariantCulture);
        if (string.IsNullOrEmpty(id)) return null;

        foreach (var choice in Choices)
            if (choice.Id == id)
                return id;

        Logger.Debug($"[UI] '{Key}' is '{id}', which is not among the declared choices.");
        return null;
    }

    /// <summary>
    ///     Writes the chosen id in the entry's real type. The ids are strings because they serve both
    ///     string and number; an Int32 receiving a Single through reflection throws, so the conversion
    ///     here is not optional.
    /// </summary>
    public void WriteChoice(string id)
    {
        if (ValueType == typeof(int))
        {
            if (int.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number)) Write(number);
            else Logger.Warning($"[UI] '{Key}': '{id}' is not a valid Int32 choice id.");

            return;
        }

        if (ValueType == typeof(float))
        {
            if (float.TryParse(id, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                Write(number);
            else Logger.Warning($"[UI] '{Key}': '{id}' is not a valid float choice id.");

            return;
        }

        Write(id);
    }

    public KeyCode ReadKey() => ReadRaw() is KeyCode key ? key : KeyCode.None;

    public void Write(object value)
    {
        if (_valueProperty == null) return;

        _valueProperty.SetValue(Entry, value);
        ConfigRegistry.MarkDirty(Category);
    }

    /// <summary>
    ///     The slider works in float; int entries receive the rounded value, otherwise reflection
    ///     throws when assigning a Single to an Int32 field.
    ///     The test is the real type, not the kind: SingleSelect is numeric too, so asking the kind
    ///     would let a float reach an Int32 field the day a numeric entry gains declared options.
    /// </summary>
    public void WriteNumber(float value)
    {
        if (ValueType == typeof(int)) Write((int)Math.Round(value));
        else Write(value);
    }

    private object ReadRaw()
    {
        try
        {
            return _valueProperty?.GetValue(Entry);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    ///     MelonLoader.dll is a real managed assembly, so GetType() returns the closed generic type and
    ///     the value type comes from the generic argument — without depending on the current value.
    /// </summary>
    internal static Type DeclaredTypeOf(MelonPreferences_Entry entry)
    {
        var type = entry.GetType();
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(MelonPreferences_Entry<>))
            return type.GetGenericArguments()[0];

        return entry.BoxedValue?.GetType();
    }

    internal static ConfigEntryKind Classify(Type type)
    {
        if (type == typeof(bool)) return ConfigEntryKind.Bool;
        if (type == typeof(float)) return ConfigEntryKind.Float;
        if (type == typeof(int)) return ConfigEntryKind.Int;
        if (type == typeof(string)) return ConfigEntryKind.Text;
        if (type == typeof(KeyCode)) return ConfigEntryKind.Keybind;

        return ConfigEntryKind.Unsupported;
    }
}
