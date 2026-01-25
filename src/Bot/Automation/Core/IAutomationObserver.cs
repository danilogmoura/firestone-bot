using System.Collections;
using MelonLoader;

namespace Firebot.Bot.Automation.Core;

public abstract class AutomationObserver
{
    protected MelonPreferences_Entry<bool> _enabledEntry;

    public abstract string SectionName { get; }

    public bool IsEnabled => _enabledEntry != null && _enabledEntry.Value;

    // We define 50 as the default "middle ground".
    // Virtual allows child classes to override it if they want.
    public virtual int Priority => 50;

    public void InitializeConfig(string filePath)
    {
        var identifier = SectionName.ToLower().Replace(" ", "_");
        var category = MelonPreferences.CreateCategory(identifier, $"{SectionName} Settings");
        category.SetFilePath(filePath);

        _enabledEntry = category.CreateEntry("Enabled", true, "Enable Module",
            "Enable this automation module");

        OnConfigure(category);
        category.SaveToFile();
    }

    protected virtual void OnConfigure(MelonPreferences_Category category)
    {
        // Optional for derived classes
    }

    public abstract bool ToogleCondition();

    public abstract IEnumerator OnNotificationTriggered();
}