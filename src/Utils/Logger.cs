using System;
using Firebot.Core;
using MelonLoader;

namespace Firebot.Utils;

public class Logger
{
    private const ConsoleColor TextColor = ConsoleColor.Gray;
    private const ConsoleColor DebugColor = ConsoleColor.Blue;

    private readonly string _logTag;

    public Logger(string logTag)
    {
        _logTag = logTag;
    }


    public void Info(string message) => MelonLogger.Msg(TextColor, $"[{_logTag}] {message}");

    public void Warning(string message) => MelonLogger.Warning($"[{_logTag}] {message}");

    public void Error(string message) => MelonLogger.Error($"[{_logTag}] {message}");

    public void Debug(string message)
    {
        if (BotSettings.DebugMode || MelonDebug.IsEnabled())
            MelonLogger.Msg(DebugColor, $"[DEBUG] [{_logTag}] {message}");
    }
}