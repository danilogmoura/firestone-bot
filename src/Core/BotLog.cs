using System;
using MelonLoader;

namespace Firebot.Core;

public static class BotLog
{
    public static void Info(string message) => MelonLogger.Msg(ConsoleColor.Cyan, message);

    public static void Warning(string message) => MelonLogger.Warning(message);

    public static void Error(string message, Exception ex = null)
    {
        MelonLogger.Error(message);
        if (ex != null)
        {
            MelonLogger.Error($"Details: {ex.Message}");
            if (BotSettings.DebugMode) MelonLogger.Error(ex.StackTrace);
        }
    }

    public static void Debug(string message)
    {
        if (BotSettings.DebugMode) MelonLogger.Msg(ConsoleColor.DarkGray, $"[DBG] {message}");
    }
}