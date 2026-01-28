using System;

namespace Firebot.Exceptions;

/// <summary>
///     Exception for errors related to missing or invalid configuration (e.g., missing paths, invalid settings).
///     <para>Use during startup, config reload, or when validating settings. Enables clear logging and fallback logic.</para>
/// </summary>
public class ConfigurationException : FirebotException
{
    /// <summary>
    ///     Creates a ConfigurationException with a standardized message.
    /// </summary>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public ConfigurationException(string contextInfo = null, string correlationId = null, Exception inner = null)
        : base("A configuration error occurred. See details for context.", contextInfo, correlationId, inner) { }

    /// <summary>
    ///     Creates a ConfigurationException with a custom message.
    /// </summary>
    /// <param name="message">Custom error message.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public ConfigurationException(string message, string contextInfo = null, string correlationId = null,
        Exception inner = null)
        : base(message, contextInfo, correlationId, inner) { }
}