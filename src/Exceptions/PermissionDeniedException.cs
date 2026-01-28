using System;

namespace Firebot.Exceptions;

/// <summary>
///     Exception for permission denied errors (e.g., unauthorized access, forbidden operation).
///     <para>
///         Use when an operation is not allowed due to insufficient permissions. Enables clear user feedback and audit
///         logging.
///     </para>
/// </summary>
public class PermissionDeniedException : FirebotException
{
    /// <summary>
    ///     Creates a PermissionDeniedException with a standardized message.
    /// </summary>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public PermissionDeniedException(string contextInfo = null, string correlationId = null, Exception inner = null)
        : base("Permission denied. See details for context.", contextInfo, correlationId, inner) { }

    /// <summary>
    ///     Creates a PermissionDeniedException with a custom message.
    /// </summary>
    /// <param name="message">Custom error message.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public PermissionDeniedException(string message, string contextInfo = null, string correlationId = null,
        Exception inner = null)
        : base(message, contextInfo, correlationId, inner) { }
}