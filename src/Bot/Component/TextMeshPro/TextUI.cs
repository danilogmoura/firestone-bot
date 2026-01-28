using System;
using Firebot.Utils;
using Il2CppTMPro;
using UnityEngine;

namespace Firebot.Bot.Component.TextMeshPro;

public class TextUI
{
    private readonly string _path;
    private MappedObjectBase _activeWrapper;

    public TextUI(string path)
    {
        _path = path;
    }

    public TimeSpan Time => TimeParser.Parse(Text);
    public double TotalSeconds => Time.TotalSeconds;

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

    public void SetColor(Color newColor)
    {
        EnsureComponentInitialized();
        if (_activeWrapper == null) return;

        if (_activeWrapper is TextMeshProUGUIWrapper ugui && ugui.Exists())
            ugui.Component.color = newColor;
        else if (_activeWrapper is TextMeshProWrapper tmpro && tmpro.Exists())
            tmpro.Component.color = newColor;
    }

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

    public void SetHighlight(bool active) => EnsureComponentInitialized();

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