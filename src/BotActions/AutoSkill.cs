using System;
using System.Collections;
using System.Collections.Generic;
using Firebot.Core;
using Firebot.Infrastructure;
using Firebot.Utilities;
using MelonLoader;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.BotActions;

public static class AutoSkill
{
    /// <summary>
    ///     Steps the combo may hold.
    ///     <para>
    ///         The limit comes from the editor: the sequence is shown as a row of chips, and the control column
    ///         fits 11 one-digit chips. Ten keeps a margin instead of depending on the exact width of a digit in
    ///         the game's font. Longer sequences are still readable in the .cfg, they are just not played.
    ///     </para>
    /// </summary>
    public const int MaxComboSteps = 10;

    /// <summary>Step written when the key is missing from the .cfg. An empty value means no steps at all.</summary>
    private const string DefaultSequence = "1";

    private static MelonPreferences_Entry<string> _comboSequence;
    private static string _lastSequence;
    private static List<Hotkey> _comboHotkeys;
    private static bool _isRunning;
    private static object _autoSkillRoutineHandle;
    private static bool _isInitialized;
    private static MelonPreferences_Entry<KeyCode> _shortcutKey;
    private static MelonPreferences_Entry<bool> _isEnabled;
    private static bool IsEnabled => _isEnabled?.Value ?? false;

    /// <summary>
    ///     Armed and actually running. The bot's pause is ignored on purpose: it is frequent and would
    ///     make the indicator flicker. Exposed so the UI does not recompose the rule on its own.
    /// </summary>
    public static bool IsActive => IsEnabled && _isRunning;

    private static KeyCode ShortcutKey =>
        Enum.IsDefined(typeof(KeyCode), _shortcutKey.Value) && _shortcutKey.Value != KeyCode.None
            ? _shortcutKey.Value
            : KeyCode.F8;

    public static void Initialize()
    {
        if (_isInitialized) return;
        var clazzName = StringUtils.Humanize(nameof(AutoSkill));
        var sectionId = clazzName.Replace(" ", "_").ToLowerInvariant();

        // The display name is the panel's, and it is the plain class name the settings themselves use.
        // The identifier keeps coming from clazzName ('auto_skill') because that is the .cfg key.
        var section = MelonPreferences.CreateCategory(sectionId, nameof(AutoSkill));
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
            DefaultSequence,
            "Combo",
            "Combo sequence as comma-separated numbers, pressed in the order given and repeated forever. " +
            "Only 1, 2 and 3 are valid steps, and an invalid step is dropped. " +
            $"At most {MaxComboSteps} steps are played; anything beyond that is ignored. " +
            "An empty value plays nothing. " +
            "EXAMPLES: '1' spams hotkey 1. '2,1,2' presses 2, then 1, then 2, and repeats. " +
            "'1,2,3,1,1' uses repetitions. Default: 1."
        );

        section.SaveToFile();
        _isInitialized = true;

        ParseComboSequence();
        Logger.Info("AutoSkill configuration initialized.");
    }

    /// <summary>
    ///     Steps the value will actually play: invalid entries dropped and the rest cut at the limit.
    ///     <para>
    ///         An empty value means no steps, not the default one. The editor writes it when the user clears
    ///         the sequence, and a combo that came back as step 1 would disagree with the empty editor.
    ///     </para>
    ///     <para>
    ///         Pure, and shared with the editor: a preview parsing on its own rules could promise a combo
    ///         different from the one that is pressed.
    ///     </para>
    /// </summary>
    public static List<int> ParseSteps(string raw, out List<string> ignored, out int overflow)
    {
        var parts = (raw ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries);

        var steps = new List<int>();

        ignored = new List<string>();
        overflow = 0;

        foreach (var part in parts)
        {
            var step = part.Trim();

            // Whitespace between commas is formatting, not a typo: not worth a line in the log.
            if (step.Length == 0) continue;

            // Checked before the value: past the limit the step is not played, so whether it is valid as well
            // makes no difference to the outcome.
            if (steps.Count == MaxComboSteps)
            {
                overflow++;
                continue;
            }

            if (!int.TryParse(step, out var idx) || idx < 1 || idx > 3)
            {
                ignored.Add($"'{step}'");
                continue;
            }

            steps.Add(idx);
        }

        return steps;
    }

    private static void ParseComboSequence()
    {
        var raw = _comboSequence?.Value;
        var steps = ParseSteps(raw, out var ignored, out var overflow);

        var hotkey1 = new Hotkey(Paths.BattleLoc.BottomSideUIDesktopLoc.LeaderPanelLoc.HotKeyOneBtn);
        var hotkey2 = new Hotkey(Paths.BattleLoc.BottomSideUIDesktopLoc.LeaderPanelLoc.HotKeyTwoBtn);
        var hotkey3 = new Hotkey(Paths.BattleLoc.BottomSideUIDesktopLoc.LeaderPanelLoc.HotKeyThreeBtn);

        _comboHotkeys = new List<Hotkey>(steps.Count);

        foreach (var step in steps)
            switch (step)
            {
                case 1:
                    _comboHotkeys.Add(hotkey1);
                    break;
                case 2:
                    _comboHotkeys.Add(hotkey2);
                    break;
                default:
                    _comboHotkeys.Add(hotkey3);
                    break;
            }

        // The raw value, not the one actually parsed: the two differ when the setting is empty and the
        // default takes over, and comparing the parsed one would make SyncSequence re-parse every frame.
        var changed = raw != _lastSequence;
        _lastSequence = raw;

        // Only when the value changed, or on the first read at boot. Start re-parses on every F8 press,
        // and the same typo does not deserve a new line each time.
        if (!changed) return;

        // Silent before: '0', '4' and 'abc' end up with nothing to click, and the only sign of it was the
        // combo never running. An empty list is not a harmless no-op either — see ComboLoop.
        if (steps.Count == 0)
        {
            // Two different situations, and the empty one is not a mistake: it is the editor's Clear, or a
            // value cleared by hand.
            if (string.IsNullOrWhiteSpace(raw))
                Logger.Warning("[AutoSkill] 'combo_sequence' is empty; there is nothing to play.");
            else
                Logger.Warning($"[AutoSkill] 'combo_sequence' holds no usable step ('{raw}'): " +
                               "only 1, 2 and 3 are valid, e.g. '1' or '2,1,2'.");

            return;
        }

        var playing = string.Join(", ", steps);

        if (overflow > 0)
            Logger.Warning($"[AutoSkill] 'combo_sequence': {overflow} step(s) past the {MaxComboSteps}-step " +
                           $"limit were ignored. Playing: {playing}.");

        // The partial version of the same silence: '1,5,2' used to become '1,2' with no sign that the '5'
        // had been thrown away.
        if (ignored.Count > 0)
            Logger.Warning($"[AutoSkill] 'combo_sequence': ignored {string.Join(", ", ignored)}; " +
                           $"only 1, 2 and 3 are valid steps. Playing: {playing}.");
    }

    private static void Start()
    {
        if (!IsEnabled || _isRunning) return;

        // Read the sequence here as well, and not only at boot: it is editable from the panel while the
        // game runs, so the value parsed at Initialize is stale from the first edit onwards.
        ParseComboSequence();

        // An empty combo is not a harmless no-op: ComboLoop would spin forever on the main thread and
        // freeze the game. The refusal is already explained in the log by ParseComboSequence.
        if (_comboHotkeys.Count == 0) return;

        _isRunning = true;
        _autoSkillRoutineHandle = MelonCoroutines.Start(ComboLoop());
    }

    /// <summary>
    ///     Applies a 'combo_sequence' edited in the panel while the game runs. Rebuilding the list swaps the
    ///     reference the generator iterates, so the change lands at the start of the next cycle and never in
    ///     the middle of one.
    /// </summary>
    private static void SyncSequence()
    {
        if (_comboSequence?.Value == _lastSequence) return;

        ParseComboSequence();

        // The new value has no usable step. Keeping the old list would ignore what the user typed, and
        // leaving the generator with nothing to click is what freezes the game, so the combo stops.
        if (_comboHotkeys.Count > 0 || !_isRunning) return;

        Logger.Warning("[AutoSkill] Combo stopped: the sequence was changed to a value with nothing to play.");
        Stop();
    }

    public static void Update()
    {
        // Before the early returns below: the value can be edited from the panel whether the combo is
        // running or not.
        SyncSequence();

        // Its own hotkey, being recorded, belongs to the capture and not to this action: pressing it to
        // bind it must not also start or stop AutoSkill (see HotkeyGate).
        if (HotkeyGate.IsCapturing) return;

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
        if (_autoSkillRoutineHandle != null) MelonCoroutines.Stop(_autoSkillRoutineHandle);
        _autoSkillRoutineHandle = null;
    }

    private static IEnumerator ComboLoop()
    {
        while (_isRunning)
        {
            // Nothing to click means nothing to wait for: as a bare foreach this generator never yields,
            // and Unity runs a generator that never yields to completion inside a single frame — the main
            // thread never comes back. Start refuses to run in this state; this is the guard that holds if
            // the list ever empties while running.
            if (_comboHotkeys.Count == 0)
            {
                _isRunning = false;
                _autoSkillRoutineHandle = null;
                yield break;
            }

            foreach (var hotkey in _comboHotkeys)
                yield return hotkey?.Click();
        }
    }
}