using System;
using System.Runtime.CompilerServices;

namespace Firebot.Utils;

/// <summary>
///     Utility for safe execution of actions and functions, with traceable logging.
/// </summary>
public static class SafeExecutor
{
    /// <summary>
    ///     Safely executes an action, logging any exception with traceability.
    /// </summary>
    public static void Run(Action action, Logger logger, string correlationId = null, string contextInfo = null,
        [CallerMemberName] string actionName = null)
    {
        try
        {
            action?.Invoke();
        }
        catch (Exception ex)
        {
            logger.Error(ex, correlationId, contextInfo ?? $"Action={actionName}");
            logger.Debug(ex, correlationId, contextInfo ?? $"Action={actionName}");
        }
    }

    /// <summary>
    ///     Safely executes a function, logging any exception with traceability and returning a default value in case of error.
    /// </summary>
    public static T Run<T>(Func<T> func, Logger logger, string correlationId = null, T defaultValue = default,
        string contextInfo = null, [CallerMemberName] string actionName = null)
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            logger.Error(ex, correlationId, contextInfo ?? $"Func={actionName}");
            logger.Debug(ex, correlationId, contextInfo ?? $"Func={actionName}");
            return defaultValue;
        }
    }
}