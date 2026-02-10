using System;
using Firebot.GameModel.Base;
using Firebot.Old.Core;
using Il2CppTMPro;
using UnityEngine;

namespace Firebot.GameModel.Primitives;

public class GameText : GameElement
{
    private TMP_Text _cachedComponent;

    public GameText(string path, GameElement parent = null) :
        base(path, parent) { }

    public GameText(Transform root, string path = null) : base(root, path) { }

    private TMP_Text Component
    {
        get
        {
            if (_cachedComponent != null && _cachedComponent.gameObject == null) _cachedComponent = null;
            if (_cachedComponent != null) return _cachedComponent;
            if (Root != null) Root.TryGetComponent(out _cachedComponent);
            return _cachedComponent;
        }
    }

    public string Text => GetParsedText();

    public TimeSpan Time => TimeParser.Parse(Text);

    public double TotalSeconds => Time.TotalSeconds;

    public string GetParsedText()
    {
        if (!IsVisible() || Component == null) return string.Empty;

        try
        {
            return Component.text;
        }
        catch
        {
            return string.Empty;
        }
    }

    public void SetColor(Color newColor)
    {
        if (IsVisible() && Component != null) Component.color = newColor;
    }

    public void SetOutline(Color color, float thickness = 0.2f)
    {
        if (!IsVisible() || Component == null || Component.fontSharedMaterial == null) return;

        Component.fontSharedMaterial.EnableKeyword("OUTLINE_ON");
        Component.outlineColor = color;
        Component.outlineWidth = thickness;
        Component.UpdateMeshPadding();
        Component.SetAllDirty();
    }

    public void RemoveOutline()
    {
        if (!IsVisible() || Component == null || Component.fontSharedMaterial == null) return;

        Component.outlineWidth = 0f;
        Component.fontSharedMaterial.DisableKeyword("OUTLINE_ON");
        Component.UpdateMeshPadding();
        Component.SetAllDirty();
    }
}