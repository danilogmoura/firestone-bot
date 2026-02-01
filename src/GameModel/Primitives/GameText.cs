using System;
using Firebot.Core;
using Firebot.GameModel.Base;
using Il2CppTMPro;
using UnityEngine;

namespace Firebot.GameModel.Primitives;

public class GameText : GameElement
{
    private GameElement _currentTextWrapper;

    public GameText(string path, string contextName = null, GameElement parent = null) :
        base(path, contextName, parent) { }

    public TimeSpan Time => TimeParser.Parse(GetParsedText());

    public double TotalSeconds => Time.TotalSeconds;

    public string GetParsedText()
    {
        EnsureComponentInitialized();
        if (_currentTextWrapper == null) return string.Empty;

        return _currentTextWrapper switch
        {
            GameTextWrapper<TextMeshProUGUI> ugui => ugui.Text,
            GameTextWrapper<TextMeshPro> tmpro => tmpro.Text,
            _ => string.Empty
        };
    }

    public void SetColor(Color newColor)
    {
        EnsureComponentInitialized();
        if (_currentTextWrapper == null) return;

        if (_currentTextWrapper is GameTextWrapper<TextMeshProUGUI> ugui && ugui.IsVisible())
            ugui.Component.color = newColor;
        else if (_currentTextWrapper is GameTextWrapper<TextMeshPro> tmpro && tmpro.IsVisible())
            tmpro.Component.color = newColor;
    }

    public void SetOutline(Color color, float thickness = 0.2f)
    {
        EnsureComponentInitialized();
        if (_currentTextWrapper == null) return;

        TMP_Text textComponent = null;

        if (_currentTextWrapper is GameTextWrapper<TextMeshProUGUI> ugui)
            textComponent = ugui.Component;
        else if (_currentTextWrapper is GameTextWrapper<TextMeshPro> tmpro)
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
        if (_currentTextWrapper == null) return;

        TMP_Text textComponent = null;

        if (_currentTextWrapper is GameTextWrapper<TextMeshProUGUI> ugui)
            textComponent = ugui.Component;
        else if (_currentTextWrapper is GameTextWrapper<TextMeshProUGUI> tmpro)
            textComponent = tmpro.Component;

        if (textComponent != null)
        {
            textComponent.outlineWidth = 0f;
            textComponent.fontSharedMaterial.DisableKeyword("OUTLINE_ON");
            textComponent.UpdateMeshPadding();
            textComponent.SetAllDirty();
        }
    }

    private void EnsureComponentInitialized()
    {
        if (!IsVisible() || string.IsNullOrEmpty(Path)) return;

        var ugui = new GameTextWrapper<TextMeshProUGUI>(Path, parent: Parent);
        if (ugui.IsVisible())
        {
            _currentTextWrapper = ugui;
            return;
        }

        var tmpro = new GameTextWrapper<TextMeshPro>(Path, parent: Parent);
        if (tmpro.IsVisible()) _currentTextWrapper = tmpro;
    }

    private class GameTextWrapper<T> : GameElement where T : TMP_Text
    {
        private T _cachedComponent;

        public GameTextWrapper(string path, string contextName = null, GameElement parent = null)
            : base(path, contextName, parent) { }

        public T Component
        {
            get
            {
                if (_cachedComponent == null)
                    _cachedComponent = GetComponent<T>();
                return _cachedComponent;
            }
        }

        public string Text
        {
            get
            {
                if (!IsVisible()) return string.Empty;

                var comp = Component;
                if (comp == null || !comp.isActiveAndEnabled) return string.Empty;
                return comp.GetParsedText();
            }
        }
    }
}