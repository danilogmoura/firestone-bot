using System;
using System.Collections;

namespace Firebot.Core;

/// <summary>
///     Drives a coroutine so that a failing step stops the feature instead of ending the loop behind the mod's
///     back.
///     <para>
///         AutoSkill and AutoUpgrade start their loops straight from MelonCoroutines. An exception escaping one of
///         their steps is caught by Unity's scheduler, which logs a stack trace and kills the coroutine while the
///         mod's own flags still say the feature is running: the badge claims "on" with nothing behind it, and the
///         hotkey needs two presses to recover.
///     </para>
///     <para>
///         A try/catch cannot do this from the inside — <c>yield return</c> is not allowed in a try that has a
///         catch — so the routine has to be driven from outside, one step at a time, which is what this does. Like
///         <c>BotManager.RunSafe</c>, it only sees what the routine it drives throws between its own steps; an
///         exception inside a nested iterator belongs to Unity's scheduler.
///     </para>
/// </summary>
public static class CoroutineGuard
{
    /// <summary>
    ///     Runs <paramref name="routine" />, and on a failing step reports it with <paramref name="context" /> and
    ///     calls <paramref name="onFailure" /> before ending.
    /// </summary>
    public static IEnumerator Run(IEnumerator routine, string context, Action onFailure = null)
    {
        if (routine == null)
        {
            Logger.Error($"[FAILED] {context} returned null routine.");
            yield break;
        }

        while (true)
        {
            bool movedNext;

            try
            {
                movedNext = routine.MoveNext();
            }
            catch (Exception e)
            {
                Logger.Error($"[FAILED] {context} threw: {e.GetType().Name} - {e.Message}");
                onFailure?.Invoke();
                yield break;
            }

            if (!movedNext) yield break;
            yield return routine.Current;
        }
    }
}
