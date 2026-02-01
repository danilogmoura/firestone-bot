using System;
using System.Runtime.CompilerServices;
using Firebot.Core;
using Firebot.GameModel.Base;
using Il2CppTMPro;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.GameModel.TMProComponents;

public class TextDisplay
{
    private readonly string _path;
    private readonly Logger Log;
    private MappedObjectBase _activeWrapper;

    public TextDisplay(string path)
    {
        _path = path;
        Log = new Logger(nameof(TextDisplay));
    }

    /// <summary>
    ///     Parses the text content as a TimeSpan safely. Returns TimeSpan.Zero on error.
    /// </summary>
    public TimeSpan Time => RunSafe(() => TimeParser.Parse(Text), TimeSpan.Zero);

    public double TotalSeconds => Time.TotalSeconds;

    /// <summary>
    ///     Gets the current text content safely. Returns string.Empty on error.
    /// </summary>
    public string Text => RunSafe(() =>
    {
        EnsureComponentInitialized();
        if (_activeWrapper == null) return string.Empty;

        return _activeWrapper switch
        {
            TextMeshProUGUIWrapper ugui => ugui.Text,
            TextMeshProWrapper tmpro => tmpro.Text,
            _ => string.Empty
        };
    }, string.Empty);

    private void RunSafe(Action action, [CallerMemberName] string actionName = null) =>
        SafeExecutor.Run(action, Log, BotContext.CorrelationId, _path, actionName);

    private T RunSafe<T>(Func<T> func, T defaultValue = default, [CallerMemberName] string actionName = null) =>
        SafeExecutor.Run(func, Log, BotContext.CorrelationId, _path, defaultValue, actionName);

    public void SetColor(Color newColor) => RunSafe(() =>
    {
        EnsureComponentInitialized();
        if (_activeWrapper == null) return;

        if (_activeWrapper is TextMeshProUGUIWrapper ugui && ugui.Exists())
            ugui.Component.color = newColor;
        else if (_activeWrapper is TextMeshProWrapper tmpro && tmpro.Exists())
            tmpro.Component.color = newColor;
    });

    public void SetOutline(Color color, float thickness = 0.2f) => RunSafe(() =>
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
    });

    public void RemoveOutline() => RunSafe(() =>
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
    });

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