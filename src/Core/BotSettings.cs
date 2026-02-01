using System;
using System.IO;
using Firebot.Automation.Core;
using MelonLoader;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.Core;

public static class BotSettings
{
    private static readonly Logger Log = new(nameof(BotSettings));

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
            "Determines if the bot logic should be initialized and started automatically upon game launch.");

        _startBotDelay = _category.CreateEntry("start_bot_delay", 10.0f, "Start Bot Delay",
            "The initial cooldown (in seconds) before the bot begins execution." +
            "\nUseful for preventing conflicts while Unity is still loading the initial scene." +
            "\nClamped between 10.0 and 120.0 seconds.");

        _scanInterval = _category.CreateEntry("scan_interval", 2.0f, "Scan Interval",
            "The interval (in seconds) between each BotManager verification cycle." +
            "\nLower values make the bot more responsive but may impact FPS performance." +
            "\nClamped between 1.0 and 3600.0 seconds.");

        _interactionDelay = _category.CreateEntry("interaction_delay", 1.0f, "Interaction Delay",
            "The delay (in seconds) between individual UI interactions (clicks, transitions)." +
            "\nEnsures the game processes the command before the next action is taken. " +
            "\nClamped between 0.5 and 5.0 seconds.");

        _debugMode = _category.CreateEntry("debug_mode", false, "Enable Debug Mode",
            "Enables verbose logging and StackTrace display in the console for easier bug identification.");

        _shortcutKey = _category.CreateEntry("shortcut_key", KeyCode.F7, "Shortcut Key",
            "The physical key used to manually toggle the bot's execution state during gameplay.");

        _category.SaveToFile();
        AutomationHandler.AutoRegister(configPath);
        Log.Info($"System Initialized. Configuration: {configPath}");
    }
}