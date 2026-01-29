using System;
using Firebot.Core;
using Firebot.Exceptions;
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

    private static string ComposeMessage(string level, string logTag, string message, string correlationId = null,
        string contextInfo = null, DateTime? timestamp = null)
    {
        var ts = (timestamp ?? DateTime.UtcNow).ToString("O");
        var json =
            $"{{\"message\":\"{EscapeJson(message)}\",\"correlationId\":\"{EscapeJson(correlationId ?? "-")}\",\"context\":\"{EscapeJson(contextInfo ?? "-")}\",\"timestamp\":\"{ts}\"}}";
        return $"[{level}] [{logTag}] {json}";
    }

    private static string EscapeJson(string value) => string.IsNullOrEmpty(value)
        ? ""
        : value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");

    // Info
    public void Info(string message, string correlationId = null, string contextInfo = null)
        => MelonLogger.Msg(TextColor, ComposeMessage("INFO", _logTag, message, correlationId, contextInfo));

    public void Info(Exception ex, string correlationId = null, string contextInfo = null)
        => MelonLogger.Msg(TextColor, ComposeExceptionMessage("INFO", ex, correlationId, contextInfo));

    // Warning
    public void Warning(string message, string correlationId = null, string contextInfo = null)
        => MelonLogger.Warning(ComposeMessage("WARN", _logTag, message, correlationId, contextInfo));

    public void Warning(Exception ex, string correlationId = null, string contextInfo = null)
        => MelonLogger.Warning(ComposeExceptionMessage("WARN", ex, correlationId, contextInfo));

    // Error
    public void Error(string message, string correlationId = null, string contextInfo = null)
        => MelonLogger.Error(ComposeMessage("ERROR", _logTag, message, correlationId, contextInfo));

    public void Error(Exception ex, string correlationId = null, string contextInfo = null)
        => MelonLogger.Error(ComposeExceptionMessage("ERROR", ex, correlationId, contextInfo));

    // Debug
    public void Debug(string message, string correlationId = null, string contextInfo = null)
    {
        if (BotSettings.DebugMode || MelonDebug.IsEnabled())
            MelonLogger.Msg(DebugColor, ComposeMessage("DEBUG", _logTag, message, correlationId, contextInfo));
    }

    public void Debug(Exception ex, string correlationId = null, string contextInfo = null)
    {
        if (BotSettings.DebugMode || MelonDebug.IsEnabled())
            MelonLogger.Msg(DebugColor, ComposeExceptionMessage("DEBUG", ex, correlationId, contextInfo));
    }

    // Structured event log (for automation/component events)
    public void Event(string eventName, string result, string correlationId = null, string contextInfo = null)
    {
        var msg = $"[Event] Name={eventName} | Result={result}";
        MelonLogger.Msg(TextColor, ComposeMessage("EVENT", _logTag, msg, correlationId, contextInfo));
    }

    // Helper for exception logging
    private string ComposeExceptionMessage(string level, Exception ex, string correlationId, string contextInfo)
    {
        var msg = $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
        DateTime? ts = null;
        if (ex is FirebotException fbEx)
        {
            correlationId ??= fbEx.CorrelationId;
            contextInfo ??= fbEx.ContextInfo;
            ts = fbEx.Timestamp;
            msg +=
                $"\n[FirebotException] correlationId={fbEx.CorrelationId} | context={fbEx.ContextInfo} | timestamp={fbEx.Timestamp:O}";
        }

        return ComposeMessage(level, _logTag, msg, correlationId, contextInfo, ts);
    }
}