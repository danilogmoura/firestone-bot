using System;

namespace Firebot.Exceptions;

/// <summary>
///     Base exception for all Firebot errors, ensuring traceability and context.
///     <para>
///         All custom exceptions should inherit from this class to guarantee correlationId, timestamp, and contextInfo
///         are always available for logging and debugging.
///     </para>
///     <para>Use this as the root for all exceptions that require audit trails and structured error handling.</para>
/// </summary>
public abstract class FirebotException : Exception
{
    /// <summary>
    ///     Creates a FirebotException with a standardized generic message.
    /// </summary>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    protected FirebotException(string contextInfo = null, string correlationId = null, Exception inner = null)
        : base("An error occurred in Firebot. See details for context.", inner)
    {
        CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        Timestamp = DateTime.UtcNow;
        ContextInfo = contextInfo;
    }

    /// <summary>
    ///     Creates a FirebotException with a custom message.
    /// </summary>
    /// <param name="message">Custom error message.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    protected FirebotException(string message, string contextInfo = null, string correlationId = null,
        Exception inner = null)
        : base(message, inner)
    {
        CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        Timestamp = DateTime.UtcNow;
        ContextInfo = contextInfo;
    }

    /// <summary>
    ///     Unique identifier for correlating this exception with a specific flow or operation.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    ///     UTC timestamp when the exception was created.
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    ///     Additional context information for debugging and logging.
    /// </summary>
    public string ContextInfo { get; }
}