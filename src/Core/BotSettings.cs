using System;
using System.IO;
using Firebot.Bot.Automation.Core;
using MelonLoader;
using UnityEngine;

namespace Firebot.Core;

public static class BotSettings
{
    private static MelonPreferences_Category _category;

    // Globais
    public static MelonPreferences_Entry<bool> IsBotEnabled;
    public static MelonPreferences_Entry<float> ScanInterval;
    public static MelonPreferences_Entry<float> InteractionDelay;

    // Safe Properties
    public static float SafeScanInterval => Mathf.Max(0.1f, ScanInterval.Value);
    public static float SafeInteractionDelay => Mathf.Max(0.05f, InteractionDelay.Value);

    public static void Initialize()
    {
        var configPath = Path.Combine("UserData", "FirebotPreferences.cfg");

        _category = MelonPreferences.CreateCategory("firebot_settings", "Firebot Settings");
        _category.SetFilePath(configPath);

        IsBotEnabled = _category.CreateEntry("IsBotEnabled", true, "Enable Bot", "Enable the bot system");
        ScanInterval =
            _category.CreateEntry("ScanInterval", 5.0f, "Scan Interval", "Time between bot checks");
        InteractionDelay = _category.CreateEntry("InteractionDelay", 1.0f, "Interaction Delay",
            "Delay between UI interactions");

        _category.SaveToFile();

        AutomationHandler.AutoRegister(configPath);

        MelonLogger.Msg(ConsoleColor.DarkCyan, "[Firebot] ", ConsoleColor.Gray,
            $"System Initialized. Configuration: {configPath}");
    }
}