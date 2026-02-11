using System;
using System.Collections;
using MelonLoader;
using static Firebot.Utilities.StringUtils;

namespace Firebot.Behaviors;

public abstract class BotTask
{
    private MelonPreferences_Category _category;
    private MelonPreferences_Entry<bool> _enabledEntry;

    public virtual string SectionTitle => Humanize(GetType().Name);

    public DateTime NextRunTime { get; set; } = DateTime.MinValue;

    public virtual int Priority => 50;

    public bool IsEnabled => _enabledEntry == null || _enabledEntry.Value;

    public void InitializeConfig(string configPath)
    {
        if (_enabledEntry != null) return;

        var sectionId = SectionTitle.Replace(" ", "_").ToLowerInvariant();
        var category = MelonPreferences.CreateCategory(sectionId, $"{SectionTitle} Settings");
        category.SetFilePath(configPath);

        _enabledEntry = category.CreateEntry("enabled", true, "Enable Module",
            $"Enables or disables the {SectionTitle} automation module." +
            $"\nWhen disabled, this module will be ignored during the execution loop.");

        OnConfigure(category);
        category.SaveToFile();

        OnConfigure(_category);
        _category.SaveToFile();
    }

    protected virtual void OnConfigure(MelonPreferences_Category category) { }

    public bool IsReady() => IsEnabled && DateTime.Now >= NextRunTime;

    public abstract IEnumerator Execute();
}