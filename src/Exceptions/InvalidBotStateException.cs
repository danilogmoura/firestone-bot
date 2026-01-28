using System;

namespace Firebot.Exceptions;

/// <summary>
///     Exception for invalid or unexpected states in the bot's core logic (e.g., trying to start an action when
///     prerequisites are not met).
///     <para>Use in BotManager or core logic to log and possibly trigger recovery or fallback logic.</para>
/// </summary>
public class InvalidBotStateException : FirebotException
{
    /// <summary>
    ///     Creates an InvalidBotStateException with a standardized message.
    /// </summary>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public InvalidBotStateException(string contextInfo = null, string correlationId = null, Exception inner = null)
        : base("The bot is in an invalid state. See details for context.", contextInfo, correlationId, inner) { }

    /// <summary>
    ///     Creates an InvalidBotStateException with a custom message.
    /// </summary>
    /// <param name="message">Custom error message.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public InvalidBotStateException(string message, string contextInfo = null, string correlationId = null,
        Exception inner = null)
        : base(message, contextInfo, correlationId, inner) { }
}