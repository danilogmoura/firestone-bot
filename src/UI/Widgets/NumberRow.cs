using System;
using Firebot.UI.Config;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.UI.Widgets;

/// <summary>
///     Slider plus a value readout, for float and int entries.
///     The bounds come from the declared range, so the slider never offers a value the code would
///     clamp away.
/// </summary>
internal sealed class NumberRow : UiRow
{
    private readonly bool _isInteger;

    public NumberRow(ConfigEntry config, RectTransform rowRect) : base(config, rowRect)
    {
        // The real type, not the kind: a numeric entry with declared options is SingleSelect, and it
        // must still format as an integer.
        _isInteger = config.ValueType == typeof(int);

        BuildLabel();

        var track = UiFactory.CreateImage("slider", RowRect, UiTheme.Button, true);
        var trackRect = track.rectTransform;
        trackRect.anchorMin = new Vector2(LabelRatio + 0.03f, 0.5f);
        trackRect.anchorMax = new Vector2(0.80f, 0.5f);
        trackRect.pivot = new Vector2(0.5f, 0.5f);
        trackRect.sizeDelta = new Vector2(0f, UiTheme.SliderHeight);
        trackRect.anchoredPosition = Vector2.zero;

        // Slider drives the fill and handle anchors, but uses each one's *parent* as the container:
        // that is why each lives in its own node spanning the full track width.
        var fillArea = UiFactory.CreateNode("fillArea", trackRect);
        UiFactory.Stretch(fillArea);
        var fill = UiFactory.CreateImage("fill", fillArea, UiTheme.Accent, false);
        fill.rectTransform.sizeDelta = new Vector2(0f, -UiTheme.SliderFillInset);

        var handleArea = UiFactory.CreateNode("handleArea", trackRect);
        UiFactory.Stretch(handleArea);
        var handle = UiFactory.CreateImage("handle", handleArea, UiTheme.Text, true);
        handle.rectTransform.sizeDelta = new Vector2(UiTheme.SliderHandleWidth, -UiTheme.SliderHandleInset);

        var slider = track.gameObject.AddComponent<Slider>();
        slider.targetGraphic = track;
        slider.direction = Slider.Direction.LeftToRight;
        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        slider.wholeNumbers = config.Range.WholeNumbers;
        slider.minValue = config.Range.Min;
        slider.maxValue = config.Range.Max;
        slider.SetValueWithoutNotify(Mathf.Clamp(config.ReadNumber(), config.Range.Min, config.Range.Max));

        var readout = UiFactory.CreateLabel(RowRect, Format(slider.value), UiTheme.HintFontSize, UiTheme.TextMuted,
            TextAlignmentOptions.Right);
        var readoutRect = readout.rectTransform;
        readoutRect.anchorMin = new Vector2(0.82f, 0f);
        readoutRect.anchorMax = new Vector2(1f, 1f);
        readoutRect.offsetMin = Vector2.zero;
        readoutRect.offsetMax = new Vector2(-UiTheme.ControlInsetRight, 0f);

        slider.onValueChanged.AddListener((Action<float>)(value =>
        {
            config.WriteNumber(value);
            readout.text = Format(value);
        }));
    }

    private string Format(float value)
        => _isInteger ? ((int)Math.Round(value)).ToString() : value.ToString("0.##");
}
