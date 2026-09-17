using System;
using System.Collections.Generic;
using Firebot.UI.Config;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Firebot.Core.Logger;

namespace Firebot.UI.Widgets;

/// <summary>
///     Key capture for KeyCode entries.
///     Only one row can be listening at a time; the keyboard is read by the Tick called from
///     Main.OnUpdate, so we register no Update of our own.
/// </summary>
internal sealed class KeybindRow : UiRow
{
    private static readonly KeyCode[] AllKeys = (KeyCode[])Enum.GetValues(typeof(KeyCode));
    private static KeybindRow _listening;

    /// <summary>Refusal labels. They take the place of the value inside a 180px button, so: one or two words.</summary>
    private const string TakenNotice = "in use";

    private const string GameNotice = "game key";

    /// <summary>
    ///     Keys the game itself reacts to, taken from its own hotkey help screen: Space attacks, 1-3 are the
    ///     hero abilities and one letter opens each menu.
    ///     <para>
    ///         Binding one of these would fire the game's action every time the hotkey is used, so they are
    ///         refused. Escape is absent on purpose (it cancels the capture before this scan sees it), and so
    ///         is Alt+Enter (fullscreen): it is a combination, and one KeyCode cannot express it.
    ///     </para>
    /// </summary>
    private static readonly Dictionary<KeyCode, string> GameKeys = new()
    {
        [KeyCode.Space] = "the guardian attack",
        [KeyCode.Alpha1] = "hero ability 1",
        [KeyCode.Alpha2] = "hero ability 2",
        [KeyCode.Alpha3] = "hero ability 3",
        [KeyCode.A] = "the Alchemist",
        [KeyCode.B] = "the Bag",
        [KeyCode.C] = "the Character screen",
        [KeyCode.E] = "the Temple of Eternals",
        [KeyCode.G] = "the Guardian menu",
        [KeyCode.H] = "the Hall of Heroes",
        [KeyCode.K] = "the Arena of Kings",
        [KeyCode.L] = "the Library",
        [KeyCode.M] = "the Map",
        [KeyCode.O] = "the Oracle",
        [KeyCode.P] = "the Party screen",
        [KeyCode.Q] = "the Quests screen",
        [KeyCode.S] = "the Settings",
        [KeyCode.T] = "the Town",
        [KeyCode.U] = "the Upgrades",
        [KeyCode.X] = "the Exotic merchant"
    };

    private readonly TMP_Text _value;

    public KeybindRow(ConfigEntry config, RectTransform rowRect) : base(config, rowRect)
    {
        BuildLabel();

        var button = UiFactory.CreateButton("keybind", RowRect, string.Empty, UiTheme.Button, StartListening);
        RightAlign(button.GetComponent<RectTransform>(), UiTheme.KeybindWidth, UiTheme.ToggleHeight);

        _value = UiFactory.CreateLabel(button.transform, config.ReadKey().ToString(), UiTheme.LabelFontSize,
            UiTheme.Text, TextAlignmentOptions.Center);
        UiFactory.Stretch(_value.rectTransform, UiTheme.ControlInsetLeft);
    }

    /// <summary>
    ///     Live state: a capture is armed right now.
    ///     <para>
    ///         Only Main reads it, and only to freeze it into the gate at the start of the frame. Hotkeys
    ///         read <c>HotkeyGate.IsCapturing</c> instead, never this: the capture is consumed the moment it
    ///         is used, so this property answers "no" for the rest of the frame in which the user recorded
    ///         a key.
    ///     </para>
    /// </summary>
    public static bool IsListening => _listening != null;

    /// <summary>
    ///     Drops the capture and puts the row back to showing the stored key.
    ///     <para>
    ///         It is needed because a scene change destroys the rows while the static reference survives,
    ///         and KeybindRow is a plain C# object: it gets no help from Unity's fake-null, so IsListening
    ///         would stay true for the rest of the session — permanently blocking both panel hotkeys.
    ///     </para>
    /// </summary>
    public static void Cancel()
    {
        var row = _listening;
        _listening = null;

        // Restores the text and the color. It does nothing when the scene already took the label, which is
        // precisely the case Cancel exists for.
        if (row != null) row.StopListening();
    }

    /// <summary>A row whose label was destroyed by a scene change has nothing left to update.</summary>
    private bool IsAlive => _value != null;

    /// <summary>
    ///     Consumes the next pressed key, unless it is one the game or another setting already uses.
    ///     Called before the panel toggle.
    /// </summary>
    public static void Tick()
    {
        var row = _listening;
        if (row == null) return;

        // Second line of defence, for a host that forgets to call Cancel on a scene change: without it
        // the keyboard scan below would run every frame for the rest of the session, and the next key
        // pressed would be written into a setting through a destroyed row.
        if (!row.IsAlive)
        {
            Cancel();
            return;
        }

        // Escape cancels; it must sit before the scan, which would also find it.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            row.StopListening();
            return;
        }

        for (var i = 0; i < AllKeys.Length; i++)
        {
            var key = AllKeys[i];
            if (key == KeyCode.None) continue;

            // The mouse is not a keybind: it is how the panel itself is driven. Without this, any click
            // while recording — on another row, on a section header, on the X that closes the window —
            // would be stored as the hotkey, and would end the capture as a side effect, so the next key
            // pressed would act as a hotkey again while the user still believes it is being recorded.
            if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6) continue;

            if (!Input.GetKeyDown(key)) continue;

            var refusal = row.RefuseReason(key);
            if (refusal != null)
            {
                // Still recording on purpose: the user only has to press another key. Ending the capture
                // here would make the press look like it did nothing at all.
                row.ShowNotice(refusal);
                return;
            }

            row.Config.Write(key);
            row.StopListening();
            return;
        }
    }

    /// <summary>
    ///     Short label of the refusal, or null when the key can be bound.
    ///     <para>
    ///         Two kinds of key are refused: the ones the game itself reacts to, which would fire a game
    ///         action every time this hotkey is used, and the ones another setting of this mod is already
    ///         bound to, which would leave one of the two hotkeys unreachable. Editing the .cfg by hand is
    ///         still possible — this only guards what the capture accepts.
    ///     </para>
    /// </summary>
    private string RefuseReason(KeyCode key)
    {
        if (GameKeys.TryGetValue(key, out var action))
        {
            Logger.Warning($"[UI] '{Config.Key}': {key} opens {action} in the game; keeping {Config.ReadKey()}.");
            return GameNotice;
        }

        var owner = ConfigRegistry.FindKeybindOwner(key, Config.Key);
        if (owner == null) return null;

        Logger.Warning($"[UI] '{Config.Key}': {key} is already bound to '{owner}'; keeping {Config.ReadKey()}.");
        return TakenNotice;
    }

    /// <summary>
    ///     Puts the refusal in the button in place of the value. Short by necessity: the label is 180px wide
    ///     and neither wraps nor ellipsizes, so a sentence would run past the button.
    /// </summary>
    private void ShowNotice(string notice)
    {
        if (_value == null) return;

        _value.text = notice;
        _value.color = UiTheme.Danger;
    }

    private void StartListening()
    {
        if (_listening != null) _listening.StopListening();

        _listening = this;
        _value.text = "Press a key...";

        // The text color, not the background: Selectable's ColorTint would override the image
        // on hover.
        _value.color = UiTheme.Accent;
    }

    private void StopListening()
    {
        // Static cleared first: even if touching the label throws, the capture does not stay stuck.
        if (_listening == this) _listening = null;

        if (_value == null) return;

        _value.color = UiTheme.Text;
        _value.text = Config.ReadKey().ToString();
    }
}
