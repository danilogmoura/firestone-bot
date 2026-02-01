using MelonLoader;
using MelonLoader.Logging;

namespace Firebot.Core.Diagnostics;

public static class Logger
{
    // Instância dedicada para garantir a cor e o prefixo [Firebot]
    private static readonly MelonLogger.Instance _melon = new("Firebot", ColorARGB.Cyan);

    public static void Info(string context, string message) => _melon.Msg($"[{context}] {message}");

    public static void Warning(string context, string message) => _melon.Warning($"[{context}] {message}");

    public static void Error(string context, string message) => _melon.Error($"[{context}] {message}");

    public static void Debug(string context, string message)
    {
        if (BotSettings.DebugMode || MelonDebug.IsEnabled())
            _melon.Msg(ColorARGB.Gray, $"[DEBUG] [{context}] {message}");
    }

    public static void Info(string message) => _melon.Msg(message);
    public static void Warning(string message) => _melon.Warning(message);
}