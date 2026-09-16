using System;
using Firebot.UI.Config;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.UI.Widgets;

/// <summary>
///     Text field for string entries.
///     Uses TMP_InputField instead of the uGUI InputField on purpose: the uGUI one requires
///     UnityEngine.UI.Text, which depends on a legacy Font that may not exist in the scene.
/// </summary>
internal sealed class TextRow : UiRow
{
    public TextRow(ConfigEntry config, RectTransform rowRect) : base(config, rowRect)
    {
        BuildLabel();

        var background = UiFactory.CreateImage("input", RowRect, UiTheme.Window, true);
        var backgroundRect = background.rectTransform;
        backgroundRect.anchorMin = new Vector2(LabelRatio + 0.03f, 0.5f);
        backgroundRect.anchorMax = new Vector2(1f, 0.5f);
        backgroundRect.pivot = new Vector2(0.5f, 0.5f);
        backgroundRect.sizeDelta = new Vector2(-UiTheme.InputInsetRight, UiTheme.InputHeight);
        backgroundRect.anchoredPosition = Vector2.zero;

        // The viewport is what clips the text and provides the rect used for caret placement.
        var viewport = UiFactory.CreateNode("textArea", backgroundRect);
        UiFactory.Stretch(viewport, UiTheme.InputTextInset);
        viewport.gameObject.AddComponent<RectMask2D>();

        var text = UiFactory.CreateLabel(viewport, config.ReadText(), UiTheme.LabelFontSize, UiTheme.Text,
            TextAlignmentOptions.Left);
        UiFactory.Stretch(text.rectTransform);

        var input = background.gameObject.AddComponent<TMP_InputField>();
        input.targetGraphic = background;
        input.textViewport = viewport;
        input.textComponent = text;

        var font = UiTheme.Font;
        if (font != null) input.fontAsset = font;

        input.pointSize = UiTheme.LabelFontSize;
        input.lineType = TMP_InputField.LineType.SingleLine;
        input.characterLimit = 0;

        // No callback first: assigning the current text must not trigger a save.
        input.SetTextWithoutNotify(config.ReadText());
        input.onEndEdit.AddListener((Action<string>)(value => config.Write(value ?? string.Empty)));
    }
}
