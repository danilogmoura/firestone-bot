using System;

namespace Firebot.Exceptions;

/// <summary>
///     Exception for failures in external libraries, plugins, or system calls (e.g., UniverseLib, MelonLoader, Unity API).
///     <para>Use to wrap and rethrow exceptions from external calls, always logging with details for traceability.</para>
/// </summary>
public class ExternalDependencyException : FirebotException
{
    /// <summary>
    ///     Creates an ExternalDependencyException with a standardized message including the dependency name.
    /// </summary>
    /// <param name="dependencyName">The name of the external dependency.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public ExternalDependencyException(string dependencyName, string contextInfo = null, string correlationId = null,
        Exception inner = null)
        : base($"An error occurred with external dependency '{dependencyName}'. See details for context.", contextInfo,
            correlationId, inner)
    {
        DependencyName = dependencyName;
    }

    /// <summary>
    ///     Creates an ExternalDependencyException with a custom message and dependency name.
    /// </summary>
    /// <param name="dependencyName">The name of the external dependency.</param>
    /// <param name="message">Custom error message.</param>
    /// <param name="contextInfo">Additional context for debugging/logging.</param>
    /// <param name="correlationId">Unique identifier for traceability.</param>
    /// <param name="inner">The inner exception, if any.</param>
    public ExternalDependencyException(string dependencyName, string message, string contextInfo = null,
        string correlationId = null, Exception inner = null)
        : base(message, contextInfo, correlationId, inner)
    {
        DependencyName = dependencyName;
    }

    /// <summary>
    ///     The name of the external dependency where the error occurred.
    /// </summary>
    public string DependencyName { get; }
}