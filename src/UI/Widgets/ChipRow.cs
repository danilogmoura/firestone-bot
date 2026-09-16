using System;
using System.Collections.Generic;
using Firebot.UI.Config;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Firebot.Core.Logger;

namespace Firebot.UI.Widgets;

/// <summary>
///     Base of the chip choice rows.
///     <para>
///         Handles what is identical between choosing one option and choosing several: measuring the
///         text, positioning by cursor and painting the state. Measuring lives in a single place on
///         purpose — it is the part that depends on TMP at runtime, and two copies would drift.
///     </para>
/// </summary>
internal abstract class ChipRow : UiRow
{
    private sealed class Chip
    {
        public string Id;
        public Image Background;
        public TMP_Text Label;
    }

    private readonly List<Chip> _chips = new();

    protected ChipRow(ConfigEntry config, RectTransform rowRect) : base(config, rowRect)
    {
        BuildLabel(UiTheme.ChipLabelRatio);
    }

    /// <summary>
    ///     Called at the end of each subclass constructor, once its own state has been read.
    ///     It cannot be called from here: the base constructor runs before the subclass body.
    /// </summary>
    protected void Build()
    {
        BuildChips();
        Refresh();
    }

    protected abstract bool IsOn(string id);

    /// <summary>Applies the chip choice. Writing to the .cfg is the subclass's responsibility.</summary>
    protected abstract void ApplySelection(string id);

    /// <summary>
    ///     One row of chips, positioned by a manual x cursor. No LayoutGroup, for the same reason as
    ///     the rest of the panel: no LayoutRebuilder at runtime.
    /// </summary>
    private void BuildChips()
    {
        var area = BuildControlArea(UiTheme.ChipLabelRatio);
        var x = 0f;

        foreach (var choice in Config.Choices)
        {
            var chip = new Chip { Id = choice.Id };

            // No label in CreateButton: the text has to exist as an object in order to be measured,
            // and the width of the chip depends on that measurement.
            var button = UiFactory.CreateButton($"chip_{choice.Id}", area, string.Empty, UiTheme.Button, null);
            chip.Background = button.GetComponent<Image>();

            var label = UiFactory.CreateLabel(button.transform, choice.Label, UiTheme.ChipFontSize,
                UiTheme.TextMuted, TextAlignmentOptions.Center);
            UiFactory.Stretch(label.rectTransform);
            chip.Label = label;

            var rect = button.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(x, 0f);

            // Width estimated first and corrected after: TMP only measures once the mesh is built,
            // and to build the mesh the rect already needs a size.
            rect.sizeDelta = new Vector2(ChipWidth(choice.Label), UiTheme.ChipHeight);

            var width = UiTheme.ChipPaddingX * 2f + TextWidth(label, choice.Label);
            rect.sizeDelta = new Vector2(width, UiTheme.ChipHeight);

            // Local capture: without it the closure would read the loop variable, not this
            // iteration's.
            var id = choice.Id;
            button.onClick.AddListener((Action)(() => OnChipClicked(id)));

            _chips.Add(chip);
            x += width + UiTheme.ChipGap;
        }

        // The cursor does not know how to stop: a set of options wider than the control column runs
        // straight past it and vanishes into the scroll view's clip, with no log and no ellipsis.
        // Comparing here turns a silent visual bug into a log line.
        var needed = x - UiTheme.ChipGap;
        var available = ControlWidth();

        if (needed > available)
            Logger.Warning($"[UI] '{Config.Key}': {Config.Choices.Length} chips need about {needed:0}px but the " +
                           $"control column has about {available:0}px. The last chips will be clipped.");
    }

    /// <summary>
    ///     Width the control column will have, derived from the theme metrics.
    ///     <para>
    ///         The rect cannot be read here. Rows are built before they are placed — the anchors only
    ///         get set later, in <c>UiContentBuilder.Relayout</c>, which runs after every row already
    ///         exists. Until then the row still carries the RectTransform default (100x100), so
    ///         <c>rect.width</c> reports 100 for every row and the estimate below turns into 61px:
    ///         a bogus overflow warning on every chip row, no matter how narrow the options are.
    ///     </para>
    ///     <para>
    ///         Being a computation rather than a measurement, this is only valid for the panel width
    ///         the ratio was chosen for. A chip row in the status window would need its own width.
    ///     </para>
    /// </summary>
    private static float ControlWidth()
        => (UiTheme.WindowWidth - 2f * UiTheme.Padding) * (1f - UiTheme.ChipLabelRatio)
           - UiTheme.ControlInsetLeft - UiTheme.ControlInsetRight;

    private void OnChipClicked(string id)
    {
        ApplySelection(id);
        Refresh();
    }

    protected void Refresh()
    {
        foreach (var chip in _chips)
        {
            var on = IsOn(chip.Id);

            // Two channels instead of one: the fill changes luminance (grey → accent) and the text
            // follows (muted → full). State by hue alone would be unreadable for anyone who cannot
            // separate green from red; here the main difference is light against dark.
            var background = on ? UiTheme.Accent : UiTheme.Button;
            if (chip.Background.color != background) chip.Background.color = background;

            var text = on ? UiTheme.Text : UiTheme.TextMuted;
            if (chip.Label.color != text) chip.Label.color = text;
        }
    }
}
