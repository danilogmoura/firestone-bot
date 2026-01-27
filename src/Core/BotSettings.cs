using System;
using System.IO;
using Firebot.Bot.Automation.Core;
using Firebot.Utils;
using MelonLoader;
using UnityEngine;
using Logger = Firebot.Utils.Logger;

namespace Firebot.Core;

public static class BotSettings
{
    private static MelonPreferences_Category _category;
    private static MelonPreferences_Entry<bool> _autoStart;
    private static MelonPreferences_Entry<float> _startBotDelay;
    private static MelonPreferences_Entry<float> _scanInterval;
    private static MelonPreferences_Entry<float> _interactionDelay;
    private static MelonPreferences_Entry<bool> _debugMode;
    private static MelonPreferences_Entry<KeyCode> _shortcutKey;

    // Safe Properties
    public static bool AutoStart => _autoStart?.Value ?? false;
    public static float StartBotDelay => Mathf.Clamp(_startBotDelay.Value, 10.0f, 120.0f);
    public static float ScanInterval => Mathf.Clamp(_scanInterval.Value, 1f, 3600.0f);
    public static float InteractionDelay => Mathf.Clamp(_interactionDelay.Value, 0.5f, 5.0f);
    public static bool DebugMode => _debugMode?.Value ?? false;

    public static KeyCode ShortcutKey =>
        Enum.IsDefined(typeof(KeyCode), _shortcutKey.Value) && _shortcutKey.Value != KeyCode.None
            ? _shortcutKey.Value
            : KeyCode.F7;

    public static void Initialize()
    {
        var configPath = Path.Combine("UserData", "FirebotPreferences.cfg");

        _category = MelonPreferences.CreateCategory("firebot_settings", "Firebot Settings");
        _category.SetFilePath(configPath);

        _autoStart = _category.CreateEntry("auto_start", false, "Auto Start",
            "Determines if the bot should start automatically.");

        _startBotDelay = _category.CreateEntry("start_bot_delay", 10.0f, "Start Bot Delay",
            "The delay (in seconds) before the bot starts, primarily used when Auto Start is enabled. Clamped between 10 and 120 seconds.");

        _scanInterval = _category.CreateEntry("scan_interval", 2.0f, "Scan Interval",
            "The interval (in seconds) between bot checks, clamped between 1 and 3600 seconds.");

        _interactionDelay = _category.CreateEntry("interaction_delay", 1.0f, "Interaction Delay",
            "The delay (in seconds) between UI interactions, clamped between 0.5 and 5 seconds.");

        _debugMode = _category.CreateEntry("debug_mode", false, "Enable Debug Mode",
            "Enable debug logging for troubleshooting");

        _shortcutKey = _category.CreateEntry("shortcut_key", KeyCode.F7, "Shortcut Key",
            "The key used to toggle the bot on and off.");

        _category.SaveToFile();
        AutomationHandler.AutoRegister(configPath);
        Logger.Info(nameof(BotSettings), $"System Initialized. Configuration: {configPath}");
    }
}