using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Firebot.GameModel.Base;
using MelonLoader;
using static Firebot.Utilities.StringUtils;

namespace Firebot.Core.Tasks;

public abstract class BotTask
{
    private readonly string _className;
    private MelonPreferences_Category _category;
    private MelonPreferences_Entry<bool> _enabledEntry;
    private GameElement _notificationElement;

    protected BotTask()
    {
        _className = GetType().Name;
    }

    public virtual string SectionTitle => Humanize(GetType().Name);

    public DateTime NextRunTime { get; protected set; } = DateTime.MinValue;

    public virtual string NotificationPath => null;

    public bool IsEnabled => _enabledEntry == null || _enabledEntry.Value;

    private GameElement NotificationElement
    {
        get
        {
            if (_notificationElement != null) return _notificationElement;
            if (string.IsNullOrEmpty(NotificationPath)) return null;

            _notificationElement = new GameElement(NotificationPath);
            return _notificationElement;
        }
    }

    public void InitializeConfig(string configPath)
    {
        if (_enabledEntry != null) return;

        var sectionId = SectionTitle.Replace(" ", "_").ToLowerInvariant();
        _category = MelonPreferences.CreateCategory(sectionId, $"{SectionTitle} Settings");
        _category.SetFilePath(configPath);

        _enabledEntry = _category.CreateEntry("enabled", true, "Enable Task",
            $"Enables or disables the {SectionTitle} automation task." +
            $"\nWhen disabled, this task will be ignored during the execution loop.");

        OnConfigure(_category);
        _category.SaveToFile();
    }

    protected virtual void OnConfigure(MelonPreferences_Category category) { }

    public bool IsReady()
        => IsNotificationVisible() || (IsEnabled && DateTime.Now >= NextRunTime);

    public bool IsNotificationVisible()
        => IsEnabled && NotificationElement != null && NotificationElement.IsVisible();

    public abstract IEnumerator Execute();

    protected void Debug(string message, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        => Logger.Debug($"[{_className}::{member}:{line}] {message}");
}