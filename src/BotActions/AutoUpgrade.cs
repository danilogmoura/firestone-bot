using System;
using System.Collections;
using System.Collections.Generic;
using Firebot.Core;
using Firebot.GameModel.Base;
using Firebot.GameModel.Features.MainScene;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using Firebot.Utilities;
using MelonLoader;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.BotActions;

public static class AutoUpgrade
{
    private const float HoldPerButtonSeconds = 0.5f;
    private const float GapBetweenButtonsSeconds = 0.5f;
    private const float IdlePollSeconds = 0.5f;
    private static readonly WaitForSeconds GapBetweenButtonsWait = new(GapBetweenButtonsSeconds);
    private static readonly WaitForSeconds IdlePollWait = new(IdlePollSeconds);
    private static readonly List<CachedGameButton> ButtonsBuffer = new();
    private static bool _isRunning;
    private static object _autoUpgradeRoutineHandle;
    private static bool _isInitialized;
    private static MelonPreferences_Entry<KeyCode> _shortcutKey;
    private static MelonPreferences_Entry<bool> _isEnabled;
    private static MelonPreferences_Entry<string> _upgradeTargetSlots;

    private static bool IsEnabled => _isEnabled?.Value ?? false;

    private static bool IsShortcutDisabled => _shortcutKey?.Value == KeyCode.None;

    private static KeyCode ShortcutKey =>
        Enum.IsDefined(typeof(KeyCode), _shortcutKey?.Value ?? KeyCode.F6)
            ? _shortcutKey?.Value ?? KeyCode.F6
            : KeyCode.F6;

    public static void Initialize()
    {
        if (_isInitialized) return;

        var clazzName = StringUtils.Humanize(nameof(AutoUpgrade));
        var sectionId = clazzName.Replace(" ", "_").ToLowerInvariant();

        var section = MelonPreferences.CreateCategory(sectionId, $"{clazzName} Settings");
        section.SetFilePath(BotSettings.ConfigPath);

        _shortcutKey = section.CreateEntry(
            "shortcut_key",
            KeyCode.F6,
            "Shortcut Key",
            "The physical key used to manually toggle the AutoUpgrade execution state during gameplay. Default: F6."
        );

        _isEnabled = section.CreateEntry(
            "enabled",
            false,
            "Enable AutoUpgrade",
            "Enables or disables the AutoUpgrade automation task. When disabled, this task will be ignored during the execution loop. Default: false."
        );

        _upgradeTargetSlots = section.CreateEntry(
            "upgrade_target_slots",
            "",
            "Upgrade Target Slots",
            "AUTOUPGRADE TARGET SLOT CONFIGURATION. " +
            "\nThis setting controls which upgrade slots (heroes/skills) will be upgraded. " +
            "\nSLOT IDs ARE ZERO-BASED and range from 0 to 6. " +
            "\nORIENTATION: Slot numbering follows the list from top to bottom. " +
            "\nSLOT MAP: 0 = Base upgrade, 1 = Guardian, 2 to 6 = Heroes. " +
            "\nTASK PRIORITY: Main bot tasks always have priority over AutoUpgrade. " +
            "\nAUTO PAUSE/RESUME: When a main task is approaching, AutoUpgrade pauses about 30 seconds before that task runs, allows the task to execute, and then resumes automatically. " +
            "\nHOW TO USE: Enter comma-separated slot IDs to select the targets to upgrade. " +
            "\nEXAMPLES: '0,6' = only slots 0 and 6 will be upgraded. '3,1,5' = only slots 3, 1 and 5. " +
            "\nIf empty, AutoUpgrade will upgrade all visible slots. " +
            "\nInvalid values are ignored."
        );

        section.SaveToFile();
        _isInitialized = true;
        Logger.Info("AutoUpgrade configuration initialized.");
    }

    private static List<CachedGameButton> Buttons()
    {
        var ge = new GameElement(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.UpgradesList);
        ButtonsBuffer.Clear();

        var selectedTargetSlots = GetSelectedTargetSlots();

        if (selectedTargetSlots.Length == 0)
        {
            foreach (var child in ge.GetChildren())
            {
                if (child == null) continue;

                var button = "customUpgrade".Equals(child.Name)
                    ? new CachedGameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.BuyBtn, child)
                    : new CachedGameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.LvlUpBtn, child);

                ButtonsBuffer.Add(button);
            }

            return ButtonsBuffer;
        }

        foreach (var position in selectedTargetSlots)
        {
            var child = ge.GetChild(position);
            if (child == null) continue;

            var button = "customUpgrade".Equals(child.Name)
                ? new CachedGameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.BuyBtn, child)
                : new CachedGameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.LvlUpBtn, child);

            ButtonsBuffer.Add(button);
        }

        return ButtonsBuffer;
    }

    private static int[] GetSelectedTargetSlots()
    {
        if (_upgradeTargetSlots == null) return Array.Empty<int>();

        var value = _upgradeTargetSlots.Value;
        if (string.IsNullOrWhiteSpace(value)) return Array.Empty<int>();

        var parts = value.Split(',');
        var result = new List<int>(parts.Length);
        var seen = new bool[7];

        foreach (var part in parts)
        {
            var candidate = part.Trim();
            if (!int.TryParse(candidate, out var slot)) continue;
            if (slot < 0 || slot > 6) continue;
            if (seen[slot]) continue;

            seen[slot] = true;
            result.Add(slot);
        }

        return result.Count > 0 ? result.ToArray() : Array.Empty<int>();
    }

    public static void Update()
    {
        if (_shortcutKey == null)
            return;

        if (!IsEnabled && _isRunning)
        {
            Stop();
            return;
        }

        if (IsShortcutDisabled) return;
        if (!Input.GetKeyDown(ShortcutKey)) return;

        if (_isRunning)
        {
            Stop();
            return;
        }

        Start();
    }

    private static void Start()
    {
        if (IsShortcutDisabled) return;
        if (!IsEnabled) return;
        if (_isRunning) return;
        _isRunning = true;
        _autoUpgradeRoutineHandle = MelonCoroutines.Start(UpgradeLoop());
        Logger.Info("AutoUpgrade started.");
    }

    private static void Stop()
    {
        if (!_isRunning) return;
        _isRunning = false;
        if (_autoUpgradeRoutineHandle != null) MelonCoroutines.Stop(_autoUpgradeRoutineHandle);
        _autoUpgradeRoutineHandle = null;
        MelonCoroutines.Start(CloseUpgradesMenu());
        Logger.Info("AutoUpgrade stopped.");
    }

    private static IEnumerator UpgradeLoop()
    {
        var wasPaused = false;
        while (_isRunning)
        {
            var isPaused = BotManager.ShouldPauseAutoUpgrade();
            if (isPaused)
            {
                if (!wasPaused && Upgrades.IsVisible) yield return Upgrades.Close;
                wasPaused = true;
                yield return IdlePollWait;
                continue;
            }

            if (wasPaused) wasPaused = false;
            if (!Upgrades.IsVisible)
            {
                yield return new GameButton(Paths.BattleLoc.BottomSideUIDesktopLoc.MenuButtonsLoc.UpgradesBtn).Click();
                yield return Upgrades.SetUpgradeLevel();
                yield return IdlePollWait;
            }

            var buttons = Buttons();
            if (buttons.Count == 0)
            {
                yield return null;
                continue;
            }

            foreach (var buyUpgradeBtn in buttons)
            {
                if (BotManager.ShouldPauseAutoUpgrade()) break;

                Logger.Debug($"Name: {buyUpgradeBtn.Name}, IsVisible: {buyUpgradeBtn.IsVisible()}");
                yield return buyUpgradeBtn.HoldButton(HoldPerButtonSeconds);
                yield return GapBetweenButtonsWait;
            }
        }
    }

    private static IEnumerator CloseUpgradesMenu()
    {
        if (!Upgrades.IsVisible) yield break;
        yield return Upgrades.Close;
    }
}