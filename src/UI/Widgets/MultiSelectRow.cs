using System.Collections.Generic;
using Firebot.UI.Config;
using UnityEngine;

namespace Firebot.UI.Widgets;

/// <summary>
///     Multiple selection for string entries with options declared in ConfigRegistry.MultiChoices.
///     <para>
///         The widget is only a naming layer: the .cfg still stores "0,1" and the task keeps reading
///         the same format it already read. Changing the option set means editing the registry
///         dictionary, not this file.
///     </para>
/// </summary>
internal sealed class MultiSelectRow : ChipRow
{
    private readonly List<string> _selected = new();

    public MultiSelectRow(ConfigEntry config, RectTransform rowRect) : base(config, rowRect)
    {
        // The state must be read before Build, and the base constructor runs before this body.
        _selected.AddRange(config.ReadSelection());

        Build();
    }

    protected override bool IsOn(string id) => _selected.Contains(id);

    protected override void ApplySelection(string id)
    {
        if (!_selected.Remove(id)) _selected.Add(id);

        Config.Write(Config.EncodeSelection(_selected));
    }
}
