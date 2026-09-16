using System;
using System.IO;
using MelonLoader;
using UnityEngine;

namespace Firebot.Core;

public static class BotSettings
{
    private static MelonPreferences_Category _category;
    private static MelonPreferences_Entry<bool> _autoStart;
    private static MelonPreferences_Entry<float> _startBotDelay;
    private static MelonPreferences_Entry<float> _scanInterval;
    private static MelonPreferences_Entry<float> _interactionDelay;
    private static MelonPreferences_Entry<float> _maxTaskRuntime;
    private static MelonPreferences_Entry<bool> _debugMode;
    private static MelonPreferences_Entry<KeyCode> _shortcutKey;
    private static MelonPreferences_Entry<KeyCode> _panelKey;
    private static MelonPreferences_Entry<KeyCode> _statusKey;
    private static MelonPreferences_Entry<float> _freeSpeedupSeconds;

    // Valid ranges declared here, not scattered across the Clamp calls: the UI uses these exact same
    // constants for the sliders, so it never offers a value the code would silently clamp away.
    public const float StartBotDelayMin = 10.0f;
    public const float StartBotDelayMax = 120.0f;
    public const float ScanIntervalMin = 5.0f;
    public const float ScanIntervalMax = 3600.0f;
    public const float InteractionDelayMin = 0.5f;
    public const float InteractionDelayMax = 5.0f;
    public const float MaxTaskRuntimeMin = 10.0f;
    public const float MaxTaskRuntimeMax = 3600.0f;
    public const float FreeSpeedupSecondsMin = 0.0f;
    public const float FreeSpeedupSecondsMax = 180.0f;

    private static string _configPath;

    public static float FreeSpeedupSeconds =>
        Mathf.Clamp(_freeSpeedupSeconds.Value, FreeSpeedupSecondsMin, FreeSpeedupSecondsMax);

    public static string ConfigPath
    {
        get
        {
            if (string.IsNullOrEmpty(_configPath)) _configPath = Path.Combine("UserData", "FirebotPreferences.cfg");
            return _configPath;
        }
    }

    // Safe Properties
    public static bool AutoStart => _autoStart?.Value ?? false;
    public static float StartBotDelay => Mathf.Clamp(_startBotDelay.Value, StartBotDelayMin, StartBotDelayMax);
    public static float ScanInterval => Mathf.Clamp(_scanInterval.Value, ScanIntervalMin, ScanIntervalMax);
    public static float InteractionDelay => Mathf.Clamp(_interactionDelay.Value, InteractionDelayMin, InteractionDelayMax);
    public static float MaxTaskRuntime => Mathf.Clamp(_maxTaskRuntime.Value, MaxTaskRuntimeMin, MaxTaskRuntimeMax);
    public static bool DebugMode => _debugMode?.Value ?? false;

    public static KeyCode ShortcutKey =>
        Enum.IsDefined(typeof(KeyCode), _shortcutKey.Value) && _shortcutKey.Value != KeyCode.None
            ? _shortcutKey.Value
            : KeyCode.F7;

    public static KeyCode PanelKey =>
        Enum.IsDefined(typeof(KeyCode), _panelKey.Value) && _panelKey.Value != KeyCode.None
            ? _panelKey.Value
            : KeyCode.F1;

    public static KeyCode StatusKey =>
        Enum.IsDefined(typeof(KeyCode), _statusKey.Value) && _statusKey.Value != KeyCode.None
            ? _statusKey.Value
            : KeyCode.F2;

    /// <summary>
    ///     The firebot_settings category. It serves as the reference for the UI to identify which
    ///     categories belong to this .cfg, since MelonLoader's category/file association is internal.
    /// </summary>
    internal static MelonPreferences_Category Category => _category;

    public static void Initialize()
    {
        _category = MelonPreferences.CreateCategory("firebot_settings", "Firebot Settings");
        _category.SetFilePath(ConfigPath);

        _autoStart = _category.CreateEntry("auto_start", false, "Auto Start",
            "Determines if the bot logic should be initialized and started automatically upon game launch.");

        _startBotDelay = _category.CreateEntry("start_bot_delay", 10.0f, "Start Bot Delay",
            "The initial cooldown (in seconds) before the bot begins execution." +
            "\nUseful for preventing conflicts while Unity is still loading the initial scene." +
            "\nClamped between 10.0 and 120.0 seconds.");

        _scanInterval = _category.CreateEntry("scan_interval", 5.0f, "Scan Interval",
            "The interval (in seconds) between each BotManager verification cycle." +
            "\nLower values make the bot more responsive but may impact FPS performance." +
            "\nClamped between 5.0 and 3600.0 seconds.");

        _interactionDelay = _category.CreateEntry("interaction_delay", 1.0f, "Interaction Delay",
            "The delay (in seconds) between individual UI interactions (clicks, transitions)." +
            "\nEnsures the game processes the command before the next action is taken. " +
            "\nClamped between 0.5 and 5.0 seconds.");
        _maxTaskRuntime = _category.CreateEntry("max_task_runtime", 120.0f, "Max Task Runtime",
            "Maximum time (in seconds) a single task is allowed to run before it is aborted." +
            "\nClamped between 10.0 and 3600.0 seconds.");

        _debugMode = _category.CreateEntry("debug_mode", false, "Enable Debug Mode",
            "Enables verbose logging and StackTrace display in the console for easier bug identification.");

        _shortcutKey = _category.CreateEntry("shortcut_key", KeyCode.F7, "Shortcut Key",
            "The physical key used to manually toggle the bot's execution state during gameplay.");

        _panelKey = _category.CreateEntry("panel_key", KeyCode.F1, "Panel Key",
            "The physical key used to open and close the in-game configuration panel.");

        _statusKey = _category.CreateEntry("status_key", KeyCode.F2, "Status Key",
            "The physical key used to open and close the task status screen.");

        _freeSpeedupSeconds = _category.CreateEntry(
            "free_speedup_seconds",
            170.0f,
            "Free Speedup Threshold",
            "Some timers in the game can be sped up for free if the remaining time is below this threshold (default: 170 seconds = 2 minutes and 50 seconds). " +
            "The maximum allowed value is 180 seconds (3 minutes). " +
            "Set to 0 to disable free speedup. " +
            "Adjust this value to account for lag or future game changes. " +
            "Affects firestone researches, missions, experiments, and map reset timers. " +
            "If the remaining time is less than or equal to this value, the speedup is free (no gems required)."
        );
        _category.SaveToFile();
        Logger.Info($"System Initialized. Configuration: {ConfigPath}");
    }
}