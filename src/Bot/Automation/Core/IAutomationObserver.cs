using System;
using System.Collections;
using Firebot.Utils;
using MelonLoader;
using UnityEngine;
using static Firebot.Core.BotSettings;
using Logger = Firebot.Utils.Logger;

namespace Firebot.Bot.Automation.Core;

public abstract class AutomationObserver
{
    private MelonPreferences_Entry<bool> _enabledEntry;
    private double _nextExecutionTime = -1;

    public virtual string SectionTitle => StringUtils.Humanize(GetType().Name);

    public string SectionId => SectionTitle.ToLower().Replace(" ", "_");

    public bool IsEnabled => _enabledEntry != null && _enabledEntry.Value;

    // Lower numeric values indicate higher priority (0 = highest, 100 = lowest).
    // Virtual allows derived classes to override it if they want.
    public virtual int Priority => 50;

    public void InitializeConfig(string filePath)
    {
        if (_enabledEntry != null) return;

        var category = MelonPreferences.CreateCategory(SectionId, $"{SectionTitle} Settings");
        category.SetFilePath(filePath);

        _enabledEntry = category.CreateEntry("enabled", true, "Enable Module",
            $"Enable the {SectionTitle} automation module");

        OnConfigure(category);
        category.SaveToFile();
    }

    protected void ScheduleNextCheck(double secondsRemaining, double offsetSeconds = 0)
    {
        if (secondsRemaining <= 0)
        {
            _nextExecutionTime = -1;
            return;
        }

        _nextExecutionTime = Time.time + (secondsRemaining - offsetSeconds);

        if (DebugMode)
        {
            var t = TimeSpan.FromSeconds(secondsRemaining);
            var formatted = $"{t.Hours:D2}h {t.Minutes:D2}m {t.Seconds:D2}s";
            LogDebug($"Next execution scheduled in {formatted} ({secondsRemaining}s) (when the UI timer reaches zero)");
        }
    }

    protected void ResetSchedule() => _nextExecutionTime = -1;

    protected virtual void OnConfigure(MelonPreferences_Category category)
    {
        // Optional for derived classes
    }

    public virtual bool ShouldExecute()
    {
        if (!IsEnabled) return false;

        if (_nextExecutionTime < 0) return true;

        return Time.time >= _nextExecutionTime;
    }

    public abstract IEnumerator OnNotificationTriggered();

    protected void Log(string message) => Logger.Info(SectionTitle, message);

    protected void LogWarning(string message) => Logger.Warning(SectionTitle, message);

    protected void LogError(string message) => Logger.Error(SectionTitle, message);

    protected void LogDebug(string message) => Logger.Debug(SectionTitle, message);
}