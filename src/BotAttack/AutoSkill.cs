using System;
using System.Collections;
using System.Collections.Generic;
using Firebot.Core;
using Firebot.Infrastructure;
using Firebot.Utilities;
using MelonLoader;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.BotAttack;

public static class AutoSkill
{
    private static MelonPreferences_Entry<string> _comboSequence;

    private static List<Hotkey> _comboHotkeys;
    private static bool _isRunning;
    private static object _routineHandle;

    private static MelonPreferences_Entry<KeyCode> _shortcutKey;
    private static MelonPreferences_Entry<bool> _isEnabled;

    private static bool IsEnabled => _isEnabled?.Value ?? false;

    private static KeyCode ShortcutKey =>
        Enum.IsDefined(typeof(KeyCode), _shortcutKey.Value) && _shortcutKey.Value != KeyCode.None
            ? _shortcutKey.Value
            : KeyCode.F8;

    public static void Initialize()
    {
        var clazzName = StringUtils.Humanize(nameof(AutoSkill));
        var sectionId = clazzName.Replace(" ", "_").ToLowerInvariant();

        var section = MelonPreferences.CreateCategory(sectionId, $"{clazzName} Settings");
        section.SetFilePath(BotSettings.ConfigPath);

        _shortcutKey = section.CreateEntry(
            "shortcut_key",
            KeyCode.F8,
            "Shortcut Key",
            "The physical key used to manually toggle the AutoSkill execution state during gameplay. Default: F8."
        );

        _isEnabled = section.CreateEntry(
            "enabled",
            false,
            "Enable AutoSkill",
            "Enables or disables the AutoSkill automation task. When disabled, this task will be ignored during the execution loop. Default: false."
        );

        _comboSequence = section.CreateEntry(
            "combo_sequence",
            "1",
            "Combo Sequence",
            "Combo sequence as comma-separated numbers. Example: '1' will spam hotkey 1, '2,1,2' will execute hotkey 2, then 1, then 2, and repeat. Only values 1, 2, or 3 are valid. Default: 1."
        );

        ParseComboSequence();

        Logger.Info("AutoSkill configuration initialized.");
    }

    private static void ParseComboSequence()
    {
        var hotkey1 = new Hotkey(Paths.BattleLoc.BottomSideUIDesktopLoc.LeaderPanelLoc.HotKeyOneBtn);
        var hotkey2 = new Hotkey(Paths.BattleLoc.BottomSideUIDesktopLoc.LeaderPanelLoc.HotKeyTwoBtn);
        var hotkey3 = new Hotkey(Paths.BattleLoc.BottomSideUIDesktopLoc.LeaderPanelLoc.HotKeyThreeBtn);

        var comboStr = _comboSequence?.Value ?? "1";
        var parts = comboStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var combo = new List<Hotkey>();

        foreach (var part in parts)
            if (int.TryParse(part.Trim(), out var idx) && idx >= 1 && idx <= 3)
                switch (idx)
                {
                    case 1:
                        combo.Add(hotkey1);
                        break;
                    case 2:
                        combo.Add(hotkey2);
                        break;
                    default:
                        combo.Add(hotkey3);
                        break;
                }

        _comboHotkeys = combo.Count > 0 ? combo : new List<Hotkey>();
    }

    private static void Start()
    {
        if (!IsEnabled || _isRunning) return;
        _isRunning = true;
        _routineHandle = MelonCoroutines.Start(ComboLoop());
    }

    public static void Update()
    {
        if (!Input.GetKeyDown(ShortcutKey)) return;

        if (_isRunning)
            Stop();
        else
            Start();
    }

    private static void Stop()
    {
        if (!_isRunning) return;
        _isRunning = false;
        if (_routineHandle != null) MelonCoroutines.Stop(_routineHandle);
        _routineHandle = null;
    }

    private static IEnumerator ComboLoop()
    {
        while (_isRunning)
            foreach (var hotkey in _comboHotkeys)
                yield return hotkey?.Click();
    }
}