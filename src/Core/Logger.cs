using MelonLoader;
using MelonLoader.Logging;

namespace Firebot.Core;

public static class Logger
{
    private static readonly MelonLogger.Instance Melon = new("Firebot", ColorARGB.Cyan);

    public static void Info(string message) => Melon.Msg($"{message}");

    public static void Warning(string message) => Melon.Warning($"{message}");

    public static void Error(string message) => Melon.Error($"{message}");

    public static void Debug(string message)
    {
        if (BotSettings.DebugMode || MelonDebug.IsEnabled()) Melon.Msg(ColorARGB.Gray, $"[DEBUG] {message}");
    }
}