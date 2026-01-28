using System;

namespace Firebot.Exceptions;

/// <summary>
///     Exception for errors that should be shown to the end user in a friendly way (e.g., invalid input, operation not
///     allowed).
///     <para>Use to differentiate user-facing errors from technical errors. Enables clear user feedback and audit logging.</para>
/// </summary>
public class UserFriendlyException : FirebotException
{
    /// <summary>
    ///     Creates a UserFriendlyException with a standardized message.
    /// </summary>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public UserFriendlyException(string contextInfo = null, string correlationId = null, Exception inner = null)
        : base("A user-friendly error occurred. See details for context.", contextInfo, correlationId, inner) { }

    /// <summary>
    ///     Creates a UserFriendlyException with a custom message.
    /// </summary>
    /// <param name="message">Custom error message.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public UserFriendlyException(string message, string contextInfo = null, string correlationId = null,
        Exception inner = null)
        : base(message, contextInfo, correlationId, inner) { }
}