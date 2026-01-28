using System;

namespace Firebot.Exceptions;

/// <summary>
///     Exception for errors occurring in automation flows (e.g., failed UI interaction, unexpected state in automation
///     logic).
///     <para>Always use in automation handlers to ensure traceability via context and correlationId.</para>
///     <para>Best used when an automation step fails or behaves unexpectedly. Enables targeted logging and recovery.</para>
/// </summary>
public class AutomationException : FirebotException
{
    /// <summary>
    ///     Creates an AutomationException with a standardized message including the automation name.
    /// </summary>
    /// <param name="automationName">The name of the automation flow or handler.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public AutomationException(string automationName, string contextInfo = null, string correlationId = null,
        Exception inner = null)
        : base($"An error occurred during the automation '{automationName}'. See details for context.", contextInfo,
            correlationId, inner)
    {
        AutomationName = automationName;
    }

    /// <summary>
    ///     Creates an AutomationException with a custom message and automation name.
    /// </summary>
    /// <param name="automationName">The name of the automation flow or handler.</param>
    /// <param name="message">Custom error message.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public AutomationException(string automationName, string message, string contextInfo = null,
        string correlationId = null, Exception inner = null)
        : base(message, contextInfo, correlationId, inner)
    {
        AutomationName = automationName;
    }

    /// <summary>
    ///     The name of the automation flow or handler where the error occurred.
    /// </summary>
    public string AutomationName { get; }
}