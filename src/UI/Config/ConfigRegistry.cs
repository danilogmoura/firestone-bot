using System;
using System.Collections.Generic;
using System.Reflection;
using Firebot.Core;
using MelonLoader;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.UI.Config;

internal sealed class ConfigGroup
{
    public ConfigGroup(MelonPreferences_Category category, List<ConfigEntry> entries)
    {
        Category = category;
        Entries = entries;
        Title = string.IsNullOrWhiteSpace(category.DisplayName) ? category.Identifier : category.DisplayName;
    }

    public MelonPreferences_Category Category { get; }
    public string Title { get; }
    public List<ConfigEntry> Entries { get; }
}

/// <summary>
///     Reads the global MelonPreferences registry and returns only what belongs to our .cfg.
///     Since every task passes <c>BotSettings.ConfigPath</c> to SetFilePath, filtering by file picks
///     up everything (firebot_settings, autoskill, autoupgrade and one section per task) without the
///     UI having to know about any of them.
/// </summary>
internal static class ConfigRegistry
{
    private const float SaveDelaySeconds = 0.4f;

    private static readonly HashSet<MelonPreferences_Category> DirtyCategories = new();
    private static readonly HashSet<string> ReportedUnwritable = new();

    /// <summary>
    ///     MelonLoader keeps the category → file association in an internal field and exposes no
    ///     public API to query it (not Category.File, not File.FilePath, not
    ///     GetPrefFileFromFilePath). Since there is no way to filter without it, we reflect that one
    ///     field and compare references: same File instance = same .cfg.
    /// </summary>
    private static readonly FieldInfo CategoryFileField = typeof(MelonPreferences_Category).GetField(
        "File", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    private static bool _filterFailureLogged;

    private static bool _hasPendingSave;
    private static float _lastChangeAt;

    /// <summary>Keys of the entries seen in the current read, used to spot declarations that match nothing.</summary>
    private static readonly HashSet<string> SeenKeys = new();

    private static bool _unmatchedDeclarationsLogged;

    /// <summary>
    ///     Ranges declared for the entries the UI has to draw as a slider.
    ///     The firebot_settings ones come from the public BotSettings constants (the same source as
    ///     the Clamp). The two task ones exist only in the description text ("Use 0-3", "up to 30"),
    ///     so they are repeated here on purpose — BotTask exposes no range metadata.
    /// </summary>
    private static readonly Dictionary<string, NumberRange> DeclaredRanges = new()
    {
        ["firebot_settings/start_bot_delay"] =
            new NumberRange(BotSettings.StartBotDelayMin, BotSettings.StartBotDelayMax, false),
        ["firebot_settings/scan_interval"] =
            new NumberRange(BotSettings.ScanIntervalMin, BotSettings.ScanIntervalMax, false),
        ["firebot_settings/interaction_delay"] =
            new NumberRange(BotSettings.InteractionDelayMin, BotSettings.InteractionDelayMax, false),
        ["firebot_settings/max_task_runtime"] =
            new NumberRange(BotSettings.MaxTaskRuntimeMin, BotSettings.MaxTaskRuntimeMax, false),
        ["firebot_settings/free_speedup_seconds"] =
            new NumberRange(BotSettings.FreeSpeedupSecondsMin, BotSettings.FreeSpeedupSecondsMax, false),
        ["magic_quarters/guardian_index"] = new NumberRange(0f, 3f, true),
        ["free_pickaxes/pickaxe_claim_threshold"] = new NumberRange(1f, 30f, true)
    };

    /// <summary>
    ///     Single choice: the stored value is exactly one of the ids.
    ///     Works for a string entry (the whole id, e.g. 'asc') and for a numeric one (the id as a
    ///     number).
    /// </summary>
    private static readonly Dictionary<string, ConfigChoice[]> SingleChoices = new()
    {
        ["magic_quarters/guardian_index"] = new[]
        {
            new ConfigChoice("0", "Vermilion"),
            new ConfigChoice("1", "Grace"),
            new ConfigChoice("2", "Ankaa"),
            new ConfigChoice("3", "Azhar")
        },
        ["map_missions/mission_time_order"] = new[]
        {
            new ConfigChoice("asc", "Shorter first"),
            new ConfigChoice("desc", "Longer first")
        }
    };

    /// <summary>
    ///     Multiple choice: any subset of the ids, stored comma-separated.
    ///     Only valid for a string entry — a number cannot hold a list.
    /// </summary>
    private static readonly Dictionary<string, ConfigChoice[]> MultiChoices = new()
    {
        ["alchemist/resource_type"] = new[]
        {
            new ConfigChoice("0", "Dragon blood"),
            new ConfigChoice("1", "Strange dust"),
            new ConfigChoice("2", "Exotic coin")
        },

        // The id is the zero-based slot the .cfg stores; the label is the name the game shows for that
        // slot. Both are declared here because the file keeps its own format.
        ["auto_upgrade/upgrade_target_slots"] = new[]
        {
            new ConfigChoice("0", "Specials"),
            new ConfigChoice("1", "Guardian"),
            new ConfigChoice("2", "1"),
            new ConfigChoice("3", "2"),
            new ConfigChoice("4", "3"),
            new ConfigChoice("5", "4"),
            new ConfigChoice("6", "5")
        }
    };

    /// <summary>
    ///     Entries holding an ordered list of steps, where order and repetition count. A text field takes the
    ///     sequence and says nothing about what it understood, which is how a typo silently dropped a step,
    ///     so these get a widget that shows the steps and appends them one at a time.
    /// </summary>
    private static readonly HashSet<string> ComboSteps = new()
    {
        "auto_skill/combo_sequence"
    };

    /// <summary>
    ///     Text shown in the panel's description box, for the entries whose .cfg description does not fit it.
    ///     <para>
    ///         The description of an entry is the comment MelonLoader writes in the file, so it is written
    ///         for the file: it can carry the full manual, examples included. The box holds roughly three
    ///         lines and truncates the rest with an ellipsis, which means the useful tail — the examples —
    ///         was the part being cut. Declaring the panel text here keeps each surface in its own format,
    ///         and the file stays exactly as it was.
    ///     </para>
    /// </summary>
    private static readonly Dictionary<string, string> PanelTexts = new()
    {
        // Settings
        ["firebot_settings/start_bot_delay"] =
            "Cooldown in seconds before the bot starts.\n" +
            "Keeps it from acting while Unity is still loading the first scene.\n" +
            "Allowed range: 10 to 120.",

        ["firebot_settings/scan_interval"] =
            "Seconds between each verification cycle of the bot.\n" +
            "Lower is more responsive and costs more FPS.\n" +
            "Allowed range: 5 to 3600.",

        ["firebot_settings/interaction_delay"] =
            "Delay in seconds between individual UI interactions.\n" +
            "Gives the game time to process a command before the next one.\n" +
            "Allowed range: 0.5 to 5.",

        ["firebot_settings/free_speedup_seconds"] =
            "Timers below this many seconds can be sped up for free (no gems).\n" +
            "0 disables it; the game caps it at 180 (3 minutes).\n" +
            "Affects research, missions, experiments and map reset.",

        // Tasks
        ["alchemist/resource_type"] =
            "HOW TO USE: Toggle which resources an experiment may use.\n" +
            "'Dragon blood' = 0, 'Strange dust' = 1, 'Exotic coin' = 2.\n" +
            "No resource marked disables the task.",

        ["firestone_research/research_priority"] =
            "HOW TO USE: Type the talent ids in research order, comma-separated.\n" +
            "Ids 1-16 (top to bottom per tree); unavailable ids are skipped.\n" +
            "Empty lets the bot pick any available talent.",

        ["free_pickaxes/pickaxe_claim_threshold"] =
            "Claim only when at least this many free pickaxes are stored.\n" +
            "Set 1 to claim as soon as one is available, or 30 to wait for the maximum.",

        ["auto_skill/combo_sequence"] =
            "HOW TO USE: +1/+2/+3 append steps, Undo removes, Clear empties.\n" +
            "Steps play in order and repeat forever: 1, 2, 3 only, max 10.\n" +
            "An empty sequence plays nothing.",

        ["auto_upgrade/upgrade_target_slots"] =
            "HOW TO USE: Toggle the buttons to pick which slots are upgraded.\n" +
            "'Specials' = slot 0, 'Guardian' = slot 1, '1'-'5' = heroes 1-5.\n" +
            "Mark all 7 to upgrade every slot; empty also means all."
    };

    /// <summary>Reads the sections of our file, in the order MelonLoader registered them.</summary>
    public static List<ConfigGroup> Read()
    {
        var groups = new List<ConfigGroup>();

        SeenKeys.Clear();

        var categories = MelonPreferences.Categories;
        if (categories == null) return groups;

        for (var i = 0; i < categories.Count; i++)
        {
            var category = categories[i];
            if (!BelongsToConfigFile(category)) continue;

            var entries = ReadEntries(category);
            if (entries.Count == 0) continue;

            groups.Add(new ConfigGroup(category, entries));
        }

        // Without this, a filter failure would show up as an empty, silent panel.
        if (groups.Count == 0 && categories.Count > 0)
            Logger.Warning(
                $"[UI] None of the {categories.Count} registered categories matches '{BotSettings.ConfigPath}'.");

        ReportUnmatchedDeclarations();
        return groups;
    }

    /// <summary>
    ///     Warns about declarations that never matched an entry. A typo in one of the tables below is
    ///     otherwise silent: the entry still exists, it just loses its widget and falls back to a slider
    ///     or a text field. Reported once per session, and only when the filter actually found entries —
    ///     otherwise every declaration would look unmatched.
    /// </summary>
    private static void ReportUnmatchedDeclarations()
    {
        if (_unmatchedDeclarationsLogged || SeenKeys.Count == 0) return;
        _unmatchedDeclarationsLogged = true;

        ReportUnmatched(DeclaredRanges.Keys, nameof(DeclaredRanges));
        ReportUnmatched(SingleChoices.Keys, nameof(SingleChoices));
        ReportUnmatched(MultiChoices.Keys, nameof(MultiChoices));
        ReportUnmatched(ComboSteps, nameof(ComboSteps));
        ReportUnmatched(PanelTexts.Keys, nameof(PanelTexts));
    }

    private static void ReportUnmatched(IEnumerable<string> declared, string table)
    {
        foreach (var key in declared)
            if (!SeenKeys.Contains(key))
                Logger.Warning($"[UI] '{key}' is declared in {table} but matches no entry; check the spelling.");
    }

    /// <summary>
    ///     Key of another keybind entry of our file that is already bound to <paramref name="key" />, or
    ///     null when the key is free.
    ///     <para>
    ///         It reads the live MelonPreferences entries — the same objects BotSettings and the bot
    ///         actions read — instead of the built rows: a collapsed section still holds its rows, but the
    ///         panel may not be built at all, and the .cfg can also be edited outside the game.
    ///     </para>
    /// </summary>
    public static string FindKeybindOwner(KeyCode key, string exceptKey)
    {
        var categories = MelonPreferences.Categories;
        if (categories == null) return null;

        for (var i = 0; i < categories.Count; i++)
        {
            var category = categories[i];
            if (!BelongsToConfigFile(category)) continue;

            var entries = category.Entries;
            if (entries == null) continue;

            for (var j = 0; j < entries.Count; j++)
            {
                var entry = entries[j];
                if (entry == null || entry.IsHidden) continue;
                if (ConfigEntry.DeclaredTypeOf(entry) != typeof(KeyCode)) continue;
                if (entry.BoxedValue is not KeyCode bound || bound != key) continue;

                var owner = $"{category.Identifier}/{entry.Identifier}";
                if (owner != exceptKey) return owner;
            }
        }

        return null;
    }

    // ---- Debounced persistence ----
    // Dragging a slider fires onValueChanged every frame; writing the file at that rate would be
    // absurd. The in-memory value is updated immediately and the file is saved once the user stops
    // interacting.

    public static void MarkDirty(MelonPreferences_Category category)
    {
        if (category == null) return;

        DirtyCategories.Add(category);
        _hasPendingSave = true;
        _lastChangeAt = Time.unscaledTime;
    }

    public static void FlushPendingSaves()
    {
        if (!_hasPendingSave) return;
        if (Time.unscaledTime - _lastChangeAt < SaveDelaySeconds) return;

        _hasPendingSave = false;

        foreach (var category in DirtyCategories)
            try
            {
                category.SaveToFile();
            }
            catch (Exception e)
            {
                Logger.Warning($"[UI] Failed to save '{category.Identifier}': {e.GetType().Name} - {e.Message}");
            }

        DirtyCategories.Clear();
    }

    public static void LogUnwritable(MelonPreferences_Category category, MelonPreferences_Entry entry)
    {
        var key = $"{category.Identifier}/{entry.Identifier}";
        if (!ReportedUnwritable.Add(key)) return;

        Logger.Warning($"[UI] '{key}' has no writable Value property; showing it read-only.");
    }

    // ---- Internals ----

    /// <summary>Whether the entry is drawn by the step widget instead of the one its type implies.</summary>
    private static bool IsComboSteps(MelonPreferences_Category category, MelonPreferences_Entry entry)
        => ComboSteps.Contains($"{category.Identifier}/{entry.Identifier}");

    /// <summary>Text the panel shows for the entry, or null to fall back to the .cfg description.</summary>
    private static string ResolvePanelText(MelonPreferences_Category category, MelonPreferences_Entry entry)
        => PanelTexts.TryGetValue($"{category.Identifier}/{entry.Identifier}", out var text) ? text : null;

    private static List<ConfigEntry> ReadEntries(MelonPreferences_Category category)
    {
        var result = new List<ConfigEntry>();

        var source = category.Entries;
        if (source == null) return result;

        for (var i = 0; i < source.Count; i++)
        {
            var entry = source[i];
            if (entry == null || entry.IsHidden) continue;

            var declared = ConfigEntry.DeclaredTypeOf(entry);
            var config = new ConfigEntry(category, entry, ResolveRange(category, entry, declared),
                ResolveChoices(category, entry, declared), IsComboSteps(category, entry),
                ResolvePanelText(category, entry));

            SeenKeys.Add(config.Key);
            result.Add(config);
        }

        return result;
    }

    /// <summary>
    ///     Options declared for an entry. Without this the UI has no way to discover that
    ///     'map_missions/mission_time_order' is a choice: to MelonLoader it is just a string.
    ///     The id is what goes into the .cfg — the name exists only on screen.
    /// </summary>
    private static ChoiceSet ResolveChoices(MelonPreferences_Category category, MelonPreferences_Entry entry,
        Type declaredType)
    {
        var key = $"{category.Identifier}/{entry.Identifier}";
        var isNumeric = declaredType == typeof(int) || declaredType == typeof(float);

        if (SingleChoices.TryGetValue(key, out var single))
        {
            if (declaredType == typeof(string) || isNumeric) return new ChoiceSet(single, false);

            Logger.Warning(
                $"[UI] '{key}' declares single choices but its type is {declaredType?.Name ?? "null"}; ignoring them.");
            return null;
        }

        if (!MultiChoices.TryGetValue(key, out var multiple)) return null;

        if (declaredType == typeof(string)) return new ChoiceSet(multiple, true);

        // Without the warning this would become a chip widget that can only store one of the marked
        // ids.
        Logger.Warning(
            $"[UI] '{key}' declares multiple choices but its type is {declaredType?.Name ?? "null"}; " +
            "a number cannot hold a list. Declare it in SingleChoices instead.");
        return null;
    }

    private static NumberRange ResolveRange(MelonPreferences_Category category, MelonPreferences_Entry entry,
        Type declaredType)
    {
        var isNumeric = declaredType == typeof(float) || declaredType == typeof(int);
        if (!isNumeric) return default;

        var key = $"{category.Identifier}/{entry.Identifier}";
        if (DeclaredRanges.TryGetValue(key, out var declared)) return declared;

        // No declared range: derive a conservative window from the current value and warn in the
        // log, so the gap shows up instead of becoming a slider that accepts anything.
        var isInt = declaredType == typeof(int);
        var current = Convert.ToSingle(entry.BoxedValue ?? 0f);
        var max = current <= 0f ? (isInt ? 10f : 100f) : current * 2f;

        Logger.Debug($"[UI] No declared range for '{key}'; using 0..{max:0.##}. Add it to DeclaredRanges.");
        return new NumberRange(0f, max, isInt);
    }

    private static bool BelongsToConfigFile(MelonPreferences_Category category)
    {
        if (category == null) return false;

        if (CategoryFileField == null)
        {
            LogFilterFailure("MelonPreferences_Category no longer has a 'File' field");
            return false;
        }

        var ours = CategoryFileField.GetValue(BotSettings.Category);
        if (ours == null)
        {
            LogFilterFailure("the firebot_settings category has no config file attached");
            return false;
        }

        return ReferenceEquals(CategoryFileField.GetValue(category), ours);
    }

    private static void LogFilterFailure(string reason)
    {
        if (_filterFailureLogged) return;

        _filterFailureLogged = true;
        Logger.Error($"[UI] Cannot tell which categories belong to the bot config ({reason}); showing no sections.");
    }
}
