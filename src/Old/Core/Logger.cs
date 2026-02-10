using System;
using System.Runtime.CompilerServices;
using System.Text;
using Firebot.Core;
using MelonLoader;

namespace Firebot.Old.Core;

public class Logger
{
    private const ConsoleColor TextColor = ConsoleColor.Gray;
    private const ConsoleColor DebugColor = ConsoleColor.Blue;
    private readonly string _logTag;

    public Logger(string logTag)
    {
        _logTag = logTag;
    }

    private static string FormatLog(string message, string correlationId, string contextInfo, string action)
    {
        // Se tudo for nulo, retorna a mensagem limpa
        if (string.IsNullOrEmpty(correlationId) && string.IsNullOrEmpty(contextInfo) && string.IsNullOrEmpty(action))
            return message;

        var sb = new StringBuilder(message);
        sb.Append(" |");

        if (!string.IsNullOrEmpty(correlationId))
            sb.Append($" [Src: {correlationId}]");

        if (!string.IsNullOrEmpty(contextInfo))
            sb.Append($" [Ctx: {contextInfo}]");

        if (!string.IsNullOrEmpty(action))
            sb.Append($" [Act: {action}]");

        return sb.ToString();
    }

    public void Info(string message, string correlationId = null, string contextInfo = null,
        [CallerMemberName] string action = null)
        => MelonLogger.Msg(TextColor, $"[{_logTag}] {FormatLog(message, correlationId, contextInfo, action)}");

    public void Warning(string message, string correlationId = null, string contextInfo = null,
        [CallerMemberName] string action = null)
        => MelonLogger.Warning($"[{_logTag}] {FormatLog(message, correlationId, contextInfo, action)}");

    public void Error(string message, string correlationId = null, string contextInfo = null,
        [CallerMemberName] string action = null)
        => MelonLogger.Error($"[{_logTag}] {FormatLog(message, correlationId, contextInfo, action)}");

    public void Debug(string message, string correlationId = null, string contextInfo = null,
        [CallerMemberName] string action = null)
    {
        if (BotSettings.DebugMode || MelonDebug.IsEnabled())
            MelonLogger.Msg(DebugColor,
                $"[DEBUG] [{_logTag}] {FormatLog(message, correlationId, contextInfo, action)}");
    }

    public void Error(Exception ex, string correlationId = null, string contextInfo = null,
        [CallerMemberName] string action = null)
    {
        var msg = $"{ex.GetType().Name}: {ex.Message}";
        MelonLogger.Error($"[{_logTag}] {FormatLog(msg, correlationId, contextInfo, action)}\n{ex.StackTrace}");
    }
}