using System;
using System.Collections;
using System.Runtime.CompilerServices;
using MelonLoader;
using static Firebot.Utilities.StringUtils;

namespace Firebot.Core.Tasks;

public abstract class BotTask
{
    private readonly string _className;
    private MelonPreferences_Category _category;
    private MelonPreferences_Entry<bool> _enabledEntry;

    protected BotTask()
    {
        _className = GetType().Name;
    }

    public virtual string SectionTitle => Humanize(GetType().Name);

    public DateTime NextRunTime { get; protected set; } = DateTime.MinValue;

    public virtual int Priority => 50;

    public bool IsEnabled => _enabledEntry == null || _enabledEntry.Value;

    public void InitializeConfig(string configPath)
    {
        if (_enabledEntry != null) return;

        var sectionId = SectionTitle.Replace(" ", "_").ToLowerInvariant();
        _category = MelonPreferences.CreateCategory(sectionId, $"{SectionTitle} Settings");
        _category.SetFilePath(configPath);

        _enabledEntry = _category.CreateEntry("enabled", true, "Enable Module",
            $"Enables or disables the {SectionTitle} automation module." +
            $"\nWhen disabled, this module will be ignored during the execution loop.");

        OnConfigure(_category);
        _category.SaveToFile();

        OnConfigure(_category);
        _category.SaveToFile();
    }

    protected virtual void OnConfigure(MelonPreferences_Category category) { }

    public bool IsReady() => IsEnabled && DateTime.Now >= NextRunTime;

    public abstract IEnumerator Execute();

    protected void Debug(string message, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        => Logger.Debug($"[{_className}::{member}:{line}] {message}");
}