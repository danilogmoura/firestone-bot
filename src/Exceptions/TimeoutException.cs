using System;

namespace Firebot.Exceptions;

/// <summary>
///     Exception for timeout errors (e.g., operation took too long to complete).
///     <para>Use when an operation exceeds its allowed time. Enables targeted logging and user feedback.</para>
/// </summary>
public class TimeoutException : FirebotException
{
    /// <summary>
    ///     Creates a TimeoutException with a standardized message.
    /// </summary>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public TimeoutException(string contextInfo = null, string correlationId = null, Exception inner = null)
        : base("A timeout occurred. See details for context.", contextInfo, correlationId, inner) { }

    /// <summary>
    ///     Creates a TimeoutException with a custom message.
    /// </summary>
    /// <param name="message">Custom error message.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public TimeoutException(string message, string contextInfo = null, string correlationId = null,
        Exception inner = null)
        : base(message, contextInfo, correlationId, inner) { }
}