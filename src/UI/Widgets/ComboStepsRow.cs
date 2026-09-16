using System;
using System.Collections.Generic;
using Firebot.BotActions;
using Firebot.UI.Config;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.UI.Widgets;

/// <summary>
///     Editor for an entry holding an ordered list of steps, where order and repetition are the point
///     ('auto_skill/combo_sequence').
///     <para>
///         A text field takes the sequence without ever showing what it understood, and that silence is how a
///         typo dropped a step and how a value with nothing playable in it reached the bot. Here the steps are
///         shown as the value that will actually be played and appended one at a time, so an invalid step
///         cannot be typed at all.
///     </para>
///     <para>
///         One row, deliberately: a chip per step would read better, but it needs a taller row, and the row
///         height is the same for all 32 settings.
///     </para>
/// </summary>
internal sealed class ComboStepsRow : UiRow
{
    /// <summary>Steps the buttons offer, in the order they appear.</summary>
    private static readonly int[] StepValues = { 1, 2, 3 };

    /// <summary>
    ///     Room for the sequence. Sized for the longest value the setting can hold — ten one-digit steps, 19
    ///     characters — so the buttons never move when a step is added.
    /// </summary>
    private const float PreviewWidth = 190f;

    /// <summary>
    ///     Shown in place of the sequence when there is none. A blank box is indistinguishable from a widget
    ///     that failed to build.
    /// </summary>
    private const string EmptyPreview = "empty";

    private readonly TMP_Text _preview;
    private readonly List<Button> _stepButtons = new();
    private readonly List<int> _steps = new();
    private readonly Button _undo;
    private readonly Button _clear;

    public ComboStepsRow(ConfigEntry config, RectTransform rowRect) : base(config, rowRect)
    {
        BuildLabel(UiTheme.ChipLabelRatio);

        var area = BuildControlArea(UiTheme.ChipLabelRatio);
        var x = 0f;

        // Parsed by the bot's own function: a preview with rules of its own could show a combo different
        // from the one that gets pressed.
        _steps.AddRange(AutoSkill.ParseSteps(config.ReadText(), out _, out _));

        _preview = UiFactory.CreateLabel(area, string.Empty, UiTheme.ChipFontSize, UiTheme.Accent,
            TextAlignmentOptions.Left);
        Place(_preview.rectTransform, x, PreviewWidth);

        // The net for a .cfg edited outside the game: the widget itself never writes past the limit.
        _preview.overflowMode = TextOverflowModes.Ellipsis;

        x += PreviewWidth + UiTheme.ChipGap;

        foreach (var step in StepValues)
        {
            // Local capture: without it the closure would read the loop variable, not this iteration's.
            var value = step;

            var button = AddButton(area, $"step_{value}", $"+{value}", x, () => Append(value));
            _stepButtons.Add(button);
            x += button.GetComponent<RectTransform>().sizeDelta.x + UiTheme.ChipGap;
        }

        _undo = AddButton(area, "steps_undo", "Undo", x, Undo);
        x += _undo.GetComponent<RectTransform>().sizeDelta.x + UiTheme.ChipGap;

        _clear = AddButton(area, "steps_clear", "Clear", x, Clear);

        Refresh();
    }

    private void Append(int step)
    {
        // Refused here as well as on the button: the widget must not be able to write a value the bot would
        // then cut.
        if (_steps.Count >= AutoSkill.MaxComboSteps) return;

        _steps.Add(step);
        Write();
    }

    /// <summary>Removes the last step, down to an empty sequence, which plays nothing.</summary>
    private void Undo()
    {
        if (_steps.Count == 0) return;

        _steps.RemoveAt(_steps.Count - 1);
        Write();
    }

    /// <summary>
    ///     Empties the sequence, stored as an empty string. The bot reads that back as no steps at all, so the
    ///     preview and what gets played stay in agreement.
    /// </summary>
    private void Clear()
    {
        if (_steps.Count == 0) return;

        _steps.Clear();
        Write();
    }

    private void Write()
    {
        Config.Write(string.Join(",", _steps));
        Refresh();
    }

    /// <summary>
    ///     Publishes the sequence and re-evaluates the buttons: adding stops at the limit, and undoing and
    ///     clearing stop at an empty sequence. An action that cannot be taken is shown as a disabled button
    ///     instead of quietly doing nothing.
    /// </summary>
    private void Refresh()
    {
        var empty = _steps.Count == 0;
        _preview.text = empty ? EmptyPreview : string.Join(",", _steps);

        var colour = empty ? UiTheme.TextMuted : UiTheme.Accent;
        if (_preview.color != colour) _preview.color = colour;

        var atLimit = _steps.Count >= AutoSkill.MaxComboSteps;
        foreach (var button in _stepButtons)
            if (button.interactable == atLimit) button.interactable = !atLimit;

        var hasSteps = !empty;
        if (_undo.interactable != hasSteps) _undo.interactable = hasSteps;
        if (_clear.interactable != hasSteps) _clear.interactable = hasSteps;
    }

    /// <summary>Pins a control at the vertical centre of the row, at the x cursor.</summary>
    private static void Place(RectTransform rect, float x, float width)
    {
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.sizeDelta = new Vector2(width, UiTheme.ChipHeight);
        rect.anchoredPosition = new Vector2(x, 0f);
    }

    /// <summary>
    ///     A chip-shaped button with its label, sized from that label: estimated first and corrected after the
    ///     measurement, because TMP only measures once the mesh is built and to build the mesh the rect
    ///     already needs a size.
    /// </summary>
    private static Button AddButton(RectTransform area, string name, string text, float x, Action onClick)
    {
        var button = UiFactory.CreateButton(name, area, string.Empty, UiTheme.Button, onClick);
        var rect = button.GetComponent<RectTransform>();

        var label = UiFactory.CreateLabel(button.transform, text, UiTheme.ChipFontSize, UiTheme.Text,
            TextAlignmentOptions.Center);
        UiFactory.Stretch(label.rectTransform);

        Place(rect, x, ChipWidth(text));
        Place(rect, x, Mathf.Max(ChipWidth(text), UiTheme.ChipPaddingX * 2f + TextWidth(label, text)));

        return button;
    }
}
