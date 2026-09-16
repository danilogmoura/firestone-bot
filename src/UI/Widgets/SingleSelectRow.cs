using Firebot.UI.Config;
using UnityEngine;

namespace Firebot.UI.Widgets;

/// <summary>
///     Single selection for entries with options declared in ConfigRegistry.SingleChoices.
///     <para>
///         Clicking another chip switches the choice. Clicking the chip already lit does nothing on
///         purpose: the value underneath cannot represent "none", so allowing a deselect would leave
///         the row in a state the .cfg cannot store.
///     </para>
/// </summary>
internal sealed class SingleSelectRow : ChipRow
{
    private string _selected;

    public SingleSelectRow(ConfigEntry config, RectTransform rowRect) : base(config, rowRect)
    {
        // The state must be read before Build, and the base constructor runs before this body.
        _selected = config.ReadChoice();

        Build();
    }

    protected override bool IsOn(string id) => _selected == id;

    protected override void ApplySelection(string id)
    {
        if (_selected == id) return;

        _selected = id;
        Config.WriteChoice(id);
    }
}
