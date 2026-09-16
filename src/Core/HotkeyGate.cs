namespace Firebot.Core;

/// <summary>
///     Shares the keyboard between the mod's hotkeys and the in-game keybind capture.
///     <para>
///         A key press lasts the whole frame, so every <c>Input.GetKeyDown</c> called in that frame sees
///         the same key. Recording a hotkey onto F6 would therefore also start AutoUpgrade, and onto F1 it
///         would open or close the panel: the key belongs to the capture, not to the hotkey.
///     </para>
///     <para>
///         Whoever is recording publishes that fact here and every hotkey asks before acting. The state is
///         frozen once per frame by <c>BeginFrame</c>, and that is not a formality: the capture is consumed
///         as soon as it is used, so a hotkey asking the capture directly afterwards is told that nothing
///         is being recorded and acts on the very key the user is trying to bind.
///     </para>
/// </summary>
public static class HotkeyGate
{
    /// <summary>
    ///     True while a keybind is being recorded, held for the whole frame in which the capture ends.
    /// </summary>
    public static bool IsCapturing { get; private set; }

    /// <summary>Freezes the state for the current frame. Called by Main before any hotkey.</summary>
    public static void BeginFrame(bool capturing)
    {
        IsCapturing = capturing;
    }
}
