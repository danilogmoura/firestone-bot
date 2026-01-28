using System;
using Firebot.Utils;
using Il2CppTMPro;
using UnityEngine;

namespace Firebot.Bot.Component.TextMeshPro;

/// <summary>
///     TextDisplay provides a unified interface for interacting with both TextMeshProUGUI and TextMeshPro components in
///     Unity.
///     It automatically detects the correct component type at runtime and exposes common operations such as reading text,
///     setting color, manipulating outline, and parsing time values from the text content.
/// </summary>
public class TextDisplay
{
    private readonly string _path;
    private MappedObjectBase _activeWrapper;

    /// <summary>
    ///     Initializes a new instance of the TextDisplay class for the given UI path.
    /// </summary>
    /// <param name="path">The UI path to the text component.</param>
    public TextDisplay(string path)
    {
        _path = path;
    }

    /// <summary>
    ///     Parses the text content as a TimeSpan using the TimeParser utility.
    /// </summary>
    public TimeSpan Time => TimeParser.Parse(Text);

    /// <summary>
    ///     Gets the total seconds represented by the parsed TimeSpan from the text content.
    /// </summary>
    public double TotalSeconds => Time.TotalSeconds;

    /// <summary>
    ///     Gets the current text content from the active text component (TextMeshProUGUI or TextMeshPro).
    /// </summary>
    public string Text
    {
        get
        {
            EnsureComponentInitialized();
            if (_activeWrapper == null) return string.Empty;

            return _activeWrapper switch
            {
                TextMeshProUGUIWrapper ugui => ugui.Text,
                TextMeshProWrapper tmpro => tmpro.Text,
                _ => string.Empty
            };
        }
    }

    /// <summary>
    ///     Sets the color of the text in the active text component.
    /// </summary>
    /// <param name="newColor">The new color to apply.</param>
    public void SetColor(Color newColor)
    {
        EnsureComponentInitialized();
        if (_activeWrapper == null) return;

        if (_activeWrapper is TextMeshProUGUIWrapper ugui && ugui.Exists())
            ugui.Component.color = newColor;
        else if (_activeWrapper is TextMeshProWrapper tmpro && tmpro.Exists())
            tmpro.Component.color = newColor;
    }

    /// <summary>
    ///     Enables and sets the outline color and thickness for the text.
    /// </summary>
    /// <param name="color">The outline color.</param>
    /// <param name="thickness">The outline thickness (default is 0.2f).</param>
    public void SetOutline(Color color, float thickness = 0.2f)
    {
        EnsureComponentInitialized();
        if (_activeWrapper == null) return;

        TMP_Text textComponent = null;

        if (_activeWrapper is TextMeshProUGUIWrapper ugui)
            textComponent = ugui.Component;
        else if (_activeWrapper is TextMeshProWrapper tmpro)
            textComponent = tmpro.Component;

        if (textComponent != null)
        {
            textComponent.fontSharedMaterial.EnableKeyword("OUTLINE_ON");
            textComponent.outlineColor = color;
            textComponent.outlineWidth = thickness;
            textComponent.UpdateMeshPadding();
            textComponent.SetAllDirty();
        }
    }

    /// <summary>
    ///     Removes the outline effect from the text.
    /// </summary>
    public void RemoveOutline()
    {
        EnsureComponentInitialized();
        if (_activeWrapper == null) return;

        TMP_Text textComponent = null;

        if (_activeWrapper is TextMeshProUGUIWrapper ugui)
            textComponent = ugui.Component;
        else if (_activeWrapper is TextMeshProWrapper tmpro)
            textComponent = tmpro.Component;

        if (textComponent != null)
        {
            textComponent.outlineWidth = 0f;
            textComponent.fontSharedMaterial.DisableKeyword("OUTLINE_ON");
            textComponent.UpdateMeshPadding();
            textComponent.SetAllDirty();
        }
    }

    /// <summary>
    ///     Ensures the correct text component wrapper is initialized and active for the given path.
    /// </summary>
    private void EnsureComponentInitialized()
    {
        if (_activeWrapper != null && _activeWrapper.IsActive()) return;
        if (string.IsNullOrEmpty(_path)) return;

        var ugui = new TextMeshProUGUIWrapper(_path);
        if (ugui.IsActive())
        {
            _activeWrapper = ugui;
            return;
        }

        var tmpro = new TextMeshProWrapper(_path);
        if (tmpro.IsActive()) _activeWrapper = tmpro;
    }
}