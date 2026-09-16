using MelonLoader;
using MelonLoader.Logging;

namespace Firebot.Core;

public static class Logger
{
    private static readonly MelonLogger.Instance Melon = new("Firebot", ColorARGB.Cyan);

    /// <summary>
    ///     Single rule for "debug on": the mod's flag or MelonLoader's own debug mode.
    ///     <para>
    ///         It is exposed because <see cref="Debug" /> filters the <em>write</em>, not the
    ///         <em>computation</em>: the message is built before the call. Anything producing expensive
    ///         output (an entire table, for instance) needs to be able to ask before building it.
    ///     </para>
    /// </summary>
    public static bool IsDebugEnabled => BotSettings.DebugMode || MelonDebug.IsEnabled();

    public static void Info(string message) => Melon.Msg($"{message}");

    public static void Warning(string message) => Melon.Warning($"{message}");

    public static void Error(string message) => Melon.Error($"{message}");

    public static void Debug(string message)
    {
        if (IsDebugEnabled) Melon.Msg(ColorARGB.Gray, $"[DEBUG] {message}");
    }
}