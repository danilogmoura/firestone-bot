using System;
using System.Collections;
using Firebot.Core;
using MelonLoader;
using UnityEngine;
using static Firebot.Core.BotSettings;
using Logger = Firebot.Core.Logger;

namespace Firebot.Automation.Core;

public abstract class AutomationObserver
{
    private MelonPreferences_Entry<bool> _enabledEntry;
    private double _nextExecutionTime = -1;
    protected Logger Log;

    protected AutomationObserver()
    {
        var className = GetType().Name;
        Log = new Logger(className);
        SectionTitle = StringUtils.Humanize(className);
    }

    public virtual string SectionTitle { get; }

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
            $"Enables or disables the {SectionTitle} automation module." +
            $"\nWhen disabled, this module will be ignored during the execution loop.");

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
            Log.Debug($"Next execution scheduled in {formatted} ({secondsRemaining}s) (when the UI timer reaches zero)",
                BotContext.CorrelationId);
        }
    }

    protected void ResetSchedule() => _nextExecutionTime = -1;

    protected virtual void OnConfigure(MelonPreferences_Category category)
    {
        // Optional for derived classes
    }

    public virtual bool ShouldExecute()
    {
        Log.Debug($"IsEnabled={IsEnabled}, NextExecutionTime={_nextExecutionTime}", BotContext.CorrelationId);

        if (!IsEnabled) return false;
        if (_nextExecutionTime < 0) return true;

        return Time.time >= _nextExecutionTime;
    }

    public abstract IEnumerator OnNotificationTriggered();

    public void StartNewExecutionCycle() => BotContext.CorrelationId = $"{GetType().Name}";
}