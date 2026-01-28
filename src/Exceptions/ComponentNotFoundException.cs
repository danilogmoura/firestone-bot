using System;

namespace Firebot.Exceptions;

/// <summary>
///     Exception thrown when a required UI component (button, text, etc.) is not found in the scene.
///     <para>Use in wrappers or UI logic when a component cannot be located by path or type.</para>
///     <para>Improves traceability and enables targeted error handling for missing UI elements.</para>
/// </summary>
public class ComponentNotFoundException : FirebotException
{
    /// <summary>
    ///     Creates a ComponentNotFoundException with a standardized message including the component name.
    /// </summary>
    /// <param name="componentName">The name or identifier of the missing component.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public ComponentNotFoundException(string componentName, string contextInfo = null, string correlationId = null,
        Exception inner = null)
        : base($"Component '{componentName}' was not found. See details for context.", contextInfo, correlationId,
            inner)
    {
        ComponentName = componentName;
    }

    /// <summary>
    ///     Creates a ComponentNotFoundException with a custom message and component name.
    /// </summary>
    /// <param name="componentName">The name or identifier of the missing component.</param>
    /// <param name="message">Custom error message.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public ComponentNotFoundException(string componentName, string message, string contextInfo = null,
        string correlationId = null, Exception inner = null)
        : base(message, contextInfo, correlationId, inner)
    {
        ComponentName = componentName;
    }

    /// <summary>
    ///     The name or identifier of the missing component.
    /// </summary>
    public string ComponentName { get; }
}