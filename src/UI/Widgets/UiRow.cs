using System;
using Firebot.UI.Config;
using Il2CppTMPro;
using UnityEngine;

namespace Firebot.UI.Widgets;
/// <summary>
///     Base of the panel rows: label on the left, control on the right.
///     All positioning is anchor-based — no LayoutGroup, so no LayoutRebuilder at runtime.
/// </summary>
internal abstract class UiRow
{
    /// <summary>Fraction of the width taken by the label; the control gets the remainder.</summary>
    protected const float LabelRatio = 0.44f;

    protected UiRow(ConfigEntry config, RectTransform rowRect)
    {
        Config = config;
        RowRect = rowRect;
    }

    protected ConfigEntry Config { get; }
    protected RectTransform RowRect { get; }

    public static UiRow Create(ConfigEntry config, RectTransform rowRect)
    {
        switch (config.Kind)
        {
            case ConfigEntryKind.Bool:
                return new BoolRow(config, rowRect);
            case ConfigEntryKind.Float:
            case ConfigEntryKind.Int:
                return new NumberRow(config, rowRect);
            case ConfigEntryKind.Text:
                return new TextRow(config, rowRect);
            case ConfigEntryKind.Keybind:
                return new KeybindRow(config, rowRect);
            case ConfigEntryKind.MultiSelect:
                return new MultiSelectRow(config, rowRect);
            case ConfigEntryKind.SingleSelect:
                return new SingleSelectRow(config, rowRect);
            case ConfigEntryKind.ComboSteps:
                return new ComboStepsRow(config, rowRect);
            default:
                return new ReadOnlyRow(config, rowRect);
        }
    }

    /// <summary>
    ///     The label accepts its own fraction because the control column varies per widget:
    ///     multiple selection needs room for several options on the same line.
    /// </summary>
    protected TMP_Text BuildLabel(float ratio = LabelRatio)
    {
        var label = UiFactory.CreateLabel(RowRect, Config.Label, UiTheme.LabelFontSize, UiTheme.Text,
            TextAlignmentOptions.Left);

        var rect = label.rectTransform;
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(ratio, 1f);
        rect.offsetMin = new Vector2(UiTheme.RowInsetLeft, 0f);
        rect.offsetMax = new Vector2(-UiTheme.RowInsetRight, 0f);

        // The label is part of the hit test as well: it is the largest area of the row, and the
        // event bubbles up to the background Button anyway. Ellipsis because a long label would
        // spill into the control column instead of simply being cut.
        label.raycastTarget = true;
        label.overflowMode = TextOverflowModes.Ellipsis;
        return label;
    }

    /// <summary>Horizontal band where the control is drawn, inside the right-hand column.</summary>
    protected RectTransform BuildControlArea(float fromRatio = LabelRatio, float rightRatio = 1f,
        float inset = UiTheme.ControlInsetY)
    {
        var area = UiFactory.CreateNode("control", RowRect);
        area.anchorMin = new Vector2(fromRatio, 0f);
        area.anchorMax = new Vector2(rightRatio, 1f);
        area.offsetMin = new Vector2(UiTheme.ControlInsetLeft, inset);
        area.offsetMax = new Vector2(-UiTheme.ControlInsetRight, -inset);
        return area;
    }

    protected static void RightAlign(RectTransform rect, float width, float height,
        float margin = UiTheme.ControlMarginRight)
    {
        rect.anchorMin = new Vector2(1f, 0.5f);
        rect.anchorMax = new Vector2(1f, 0.5f);
        rect.pivot = new Vector2(1f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(-margin, 0f);
    }

    /// <summary>Width per character of the chip font. Only the fallback when the real measurement fails.</summary>
    protected static float CharWidth => UiTheme.ChipFontSize * UiTheme.ChipCharRatio;

    /// <summary>
    ///     Natural width of the text. If the font is not resolved yet the measurement comes back zero, and
    ///     the per-character estimate keeps the widget from being born with no width.
    ///     <para>
    ///         Shared instead of copied: this is the part that depends on TMP at runtime, and two copies of
    ///         it would drift apart the day the font metrics change.
    ///     </para>
    /// </summary>
    protected static float TextWidth(TMP_Text label, string text)
    {
        try
        {
            label.ForceMeshUpdate();
            if (label.preferredWidth > 0f) return label.preferredWidth;
        }
        catch (Exception)
        {
            // Measuring is a convenience: a failure here must not keep the widget from existing.
        }

        return text.Length * CharWidth;
    }

    /// <summary>Width of a chip-shaped button: the text plus the horizontal padding on both sides.</summary>
    protected static float ChipWidth(string text) => UiTheme.ChipPaddingX * 2f + text.Length * CharWidth;
}

/// <summary>Type with no dedicated widget: shows the value as text, with no editing.</summary>
internal sealed class ReadOnlyRow : UiRow
{
    public ReadOnlyRow(ConfigEntry config, RectTransform rowRect) : base(config, rowRect)
    {
        BuildLabel();

        var text = UiFactory.CreateLabel(RowRect, config.Entry.BoxedValue?.ToString() ?? "-",
            UiTheme.HintFontSize, UiTheme.TextMuted, TextAlignmentOptions.Right);

        var rect = text.rectTransform;
        rect.anchorMin = new Vector2(LabelRatio, 0f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = new Vector2(-UiTheme.ControlInsetRight, 0f);
    }
}
