using System;

namespace Firebot.Exceptions;

/// <summary>
///     Exception for errors occurring during the handling of UI or automation events (e.g., click, value change,
///     activation).
///     <para>
///         Use when an event handler fails or an event cannot be processed as expected. Enables event-level traceability
///         and logging.
///     </para>
/// </summary>
public class EventException : FirebotException
{
    /// <summary>
    ///     Creates an EventException with a standardized message including the event name.
    /// </summary>
    /// <param name="eventName">The name of the event where the error occurred.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public EventException(string eventName, string contextInfo = null, string correlationId = null,
        Exception inner = null)
        : base($"An error occurred during the event '{eventName}'. See details for context.", contextInfo,
            correlationId, inner)
    {
        EventName = eventName;
    }

    /// <summary>
    ///     Creates an EventException with a custom message and event name.
    /// </summary>
    /// <param name="eventName">The name of the event where the error occurred.</param>
    /// <param name="message">Custom error message.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public EventException(string eventName, string message, string contextInfo = null, string correlationId = null,
        Exception inner = null)
        : base(message, contextInfo, correlationId, inner)
    {
        EventName = eventName;
    }

    /// <summary>
    ///     The name of the event where the error occurred.
    /// </summary>
    public string EventName { get; }
}