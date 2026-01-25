using System;
using System.IO;
using Firebot.Bot.Automation.Core;
using MelonLoader;
using UnityEngine;

namespace Firebot.Core;

public static class BotSettings
{
    private static MelonPreferences_Category _category;

    private static MelonPreferences_Entry<bool> _enable;
    private static MelonPreferences_Entry<float> _scanInterval;
    private static MelonPreferences_Entry<float> _interactionDelay;
    private static MelonPreferences_Entry<bool> _debugMode;

    // Safe Properties
    public static bool IsEnable => _enable?.Value ?? false;
    public static float ScanInterval => Mathf.Max(0.1f, _scanInterval.Value);

    public static float InteractionDelay => Mathf.Max(0.05f, _interactionDelay.Value);
    public static bool DebugMode => _debugMode?.Value ?? false;

    public static void Initialize()
    {
        var configPath = Path.Combine("UserData", "FirebotPreferences.cfg");

        _category = MelonPreferences.CreateCategory("firebot_settings", "Firebot Settings");
        _category.SetFilePath(configPath);

        _enable = _category.CreateEntry("Enable", false, "Enable Bot", "Enable the bot system");
        _scanInterval =
            _category.CreateEntry("ScanInterval", 5.0f, "Scan Interval", "Time between bot checks");
        _interactionDelay = _category.CreateEntry("InteractionDelay", 1.0f, "Interaction Delay",
            "Delay between UI interactions");
        _debugMode = _category.CreateEntry("DebugMode", false, "Enable Debug Mode",
            "Enable debug logging for troubleshooting");

        _category.SaveToFile();

        AutomationHandler.AutoRegister(configPath);

        MelonLogger.Msg(ConsoleColor.Gray, $"System Initialized. Configuration: {configPath}");
    }
}