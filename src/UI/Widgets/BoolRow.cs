using System;
using Firebot.UI.Config;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.UI.Widgets;

/// <summary>Toggle for boolean entries.</summary>
internal sealed class BoolRow : UiRow
{
    public BoolRow(ConfigEntry config, RectTransform rowRect) : base(config, rowRect)
    {
        BuildLabel();

        var background = UiFactory.CreateImage("toggle", RowRect, UiTheme.Button, true);
        RightAlign(background.rectTransform, UiTheme.ToggleWidth, UiTheme.ToggleHeight);

        // Toggle.GraphicUpdate calls SetAlpha on the graphic: with no children, the "on" state
        // would never show.
        var check = UiFactory.CreateImage("check", background.transform, UiTheme.Accent, false);
        UiFactory.Stretch(check.rectTransform, UiTheme.CheckInset);

        var toggle = background.gameObject.AddComponent<Toggle>();
        toggle.targetGraphic = background;
        toggle.graphic = check;

        // No callback first: setting the initial value must not trigger a save.
        toggle.SetIsOnWithoutNotify(config.ReadBool());
        toggle.onValueChanged.AddListener((Action<bool>)(value => config.Write(value)));
    }
}
