using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Firebot.Core.Tasks;
using Firebot.Utilities;
using MelonLoader;
using UnityEngine;
using static Firebot.Core.BotSettings;

namespace Firebot.Core;

public static class BotManager
{
    private const double AutoUpgradeMinExecutionWindowSeconds = 30d;

    /// <summary>
    ///     Slack over the configured scan interval before the scheduler counts as not having woken up. A frame
    ///     can be late, and a wait is rounded up to one, but a scan half a minute past its due time is a stopped
    ///     clock rather than a slow frame.
    /// </summary>
    private const double ScanWakeGraceSeconds = 30d;

    private static readonly List<BotTask> Tasks = new();
    private static object _botRoutineHandle;
    private static bool _shouldPauseAutoUpgrade;
    private static BotTask _executingTask;
    private static DateTime _lastStepAt;

    /// <summary>
    ///     When the loop's sleep is due to end, or <see cref="DateTime.MaxValue" /> while it is not asleep — the
    ///     value it also starts at, before the first scan.
    /// </summary>
    private static DateTime _nextScanDueAt = DateTime.MaxValue;

    private static bool _scanWakeWarned;
    public static bool IsRunning { get; private set; }
    private static bool IsTaskExecuting { get; set; }

    public static bool ShouldPauseAutoUpgrade() => IsRunning && (_shouldPauseAutoUpgrade || IsTaskExecuting);

    /// <summary>
    ///     Stops the bot when the task it is running has stopped advancing, and reports a scan that never came
    ///     back.
    ///     <para>
    ///         <see cref="RunSafe" /> times out between the steps of the routine it drives, so a task that
    ///         never comes back from a step it handed to Unity — a nested coroutine looping on itself, such
    ///         as clicking a button that never stops being clickable — is invisible to it: the timeout is
    ///         never reached, the scan never returns, no other task ever runs, and the auto upgrade stays
    ///         paused because a task is still marked as executing. Nothing in the log says why.
    ///     </para>
    ///     <para>
    ///         This runs from <c>OnUpdate</c> instead of from a coroutine, which is what lets it see that
    ///         case at all, and it keeps ticking while every wait in the mod is stuck on scaled time.
    ///         Stopping is the way out: it drops the scheduler, and the nested routine goes with it. A
    ///         legitimate execution never trips it — the longest single wait in the mod is
    ///         <c>InteractionDelay</c>, a few seconds, against a limit measured in minutes.
    ///     </para>
    /// </summary>
    public static void WatchForStall()
    {
        if (!IsRunning) return;

        if (IsTaskExecuting)
        {
            var stalledFor = DateTime.Now - _lastStepAt;
            if (stalledFor <= TimeSpan.FromSeconds(MaxTaskRuntime)) return;

            var task = _executingTask?.SectionTitle ?? "A task";
            Logger.Error($"[FAILED] {task} has not advanced for {stalledFor.TotalSeconds:0}s " +
                         $"(limit {MaxTaskRuntime:0}s). Stopping the bot.");
            Stop();
            return;
        }

        // While the loop is asleep, a scan that never comes back is the game clock stopped rather than the bot
        // stuck: the wait is scaled time, so timeScale == 0 never resumes it, and the mod goes quiet while the
        // bot still reads as running. Warned and not stopped on purpose — this one heals itself when the clock
        // returns, and stopping would end a session the user did not ask to end. Once per sleep, so it
        // explains the silence without filling the log with it.
        var lateBy = DateTime.Now - _nextScanDueAt;
        if (lateBy <= TimeSpan.FromSeconds(ScanWakeGraceSeconds) || _scanWakeWarned) return;

        _scanWakeWarned = true;
        Logger.Warning($"[Bot] The scan has not resumed for {lateBy.TotalSeconds:0}s " +
                       $"(interval {ScanInterval:0}s). Nothing is scheduled until it does — most likely the " +
                       "game clock is stopped.");
    }

    public static void Initialize()
    {
        const string targetNamespace = "Firebot.Behaviors";
        Tasks.Clear();

        var assembly = Assembly.GetExecutingAssembly();
        var taskTypes = assembly.GetTypes()
            .Where(task => task.Namespace != null && task.Namespace.StartsWith(targetNamespace) &&
                           task.IsSubclassOf(typeof(BotTask)) && !task.IsAbstract);

        foreach (var type in taskTypes)
            try
            {
                var task = (BotTask)Activator.CreateInstance(type);
                if (task != null)
                {
                    task.InitializeConfig(ConfigPath);
                    Tasks.Add(task);
                }
            }
            catch (Exception e)
            {
                Logger.Info($"[Loader] Failed to load {type.Name}: {e.GetType().Name} - {e.Message}");
            }
    }

    public static void Start()
    {
        if (IsRunning) return;
        if (Tasks.Count == 0) Initialize();

        IsRunning = true;
        _shouldPauseAutoUpgrade = false;

        // The loop has not slept yet. A due time left over from a previous run would read as a scan that never
        // came back — during the auto start delay, which is not a scan wait at all.
        _nextScanDueAt = DateTime.MaxValue;
        _scanWakeWarned = false;

        _botRoutineHandle = MelonCoroutines.Start(BotSchedulerLoop());
        Logger.Info($"Started. Tasks loaded: {Tasks.Count(t => t.IsEnabled)}");
    }

    public static void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
        IsTaskExecuting = false;
        _shouldPauseAutoUpgrade = false;

        // Stopping a coroutine does not run its finally, so the task pinned to the top of the table has to be
        // released here too.
        _executingTask = null;
        if (_botRoutineHandle != null) MelonCoroutines.Stop(_botRoutineHandle);
        Logger.Info("Stopped.");
    }

    private static IEnumerator BotSchedulerLoop()
    {
        if (AutoStart) yield return new WaitForSeconds(StartBotDelay);

        while (IsRunning)
        {
            // Awake, so a pause already reported can be reported again on the next one.
            _scanWakeWarned = false;

            BotTask notificationTask = null;
            BotTask readyTask = null;

            // The scan is the one part of the loop nothing else guards. What a task throws is caught by RunSafe,
            // but an exception here — a game lookup failing while the UI is mid-rebuild, say — would end the
            // coroutine and, with it, the session: the bot would read as running with nothing ever scheduled
            // again, which is the same silence the stall checks exist to break. It is also the synchronous
            // part, and that is what makes it catchable: yield return is not allowed in a try that has a catch.
            try
            {
                var earliest = DateTime.MaxValue;
                var nextEnabledTaskRun = DateTime.MaxValue;

                foreach (var task in Tasks)
                {
                    // A task locked by level is enabled but cannot run. Counting its next run — MinValue, for
                    // one that never ran — would leave the auto upgrade thinking a task is always about to
                    // fire, and it would never resume while the lock lasts.
                    if (task.IsEnabled && !task.IsLevelLocked && task.NextRunTime < nextEnabledTaskRun)
                        nextEnabledTaskRun = task.NextRunTime;

                    if (notificationTask == null && task.IsNotificationVisible())
                    {
                        notificationTask = task;
                        continue;
                    }

                    if (!task.IsReady()) continue;
                    if (task.NextRunTime >= earliest) continue;

                    earliest = task.NextRunTime;
                    readyTask = task;
                }

                if (notificationTask != null) readyTask = notificationTask;

                var hasNearTask = nextEnabledTaskRun != DateTime.MaxValue &&
                                  (nextEnabledTaskRun - DateTime.Now).TotalSeconds <=
                                  AutoUpgradeMinExecutionWindowSeconds;
                _shouldPauseAutoUpgrade = notificationTask != null || readyTask != null || hasNearTask;
            }
            catch (Exception e)
            {
                Logger.Error($"[FAILED] The scan threw: {e.GetType().Name} - {e.Message}. Stopping the bot.");

                // Cleared here rather than through Stop(): that stops this very coroutine from the inside. Ending
                // the loop and lowering the flag is the same state, reached without re-entering MelonCoroutines.
                IsRunning = false;
                yield break;
            }

            if (readyTask != null)
            {
                IsTaskExecuting = true;
                _executingTask = readyTask;
                _lastStepAt = DateTime.Now;
                try
                {
                    yield return RunSafe(Watchdog.ForceClearAll($"before {readyTask.SectionTitle}"),
                        $"Watchdog cleanup before {readyTask.SectionTitle}");

                    // Stopwatch, completion line and table are all diagnostics: with debug off none of
                    // them is needed, and the console stays quiet in normal use. The guard is single for
                    // the whole block — and the stopwatch is only created when it is going to be read,
                    // because Logger.Debug filters the write but not the cost of building the output.
                    var stopwatch = Logger.IsDebugEnabled ? Stopwatch.StartNew() : null;

                    yield return RunSafe(readyTask.Execute(), $"Task {readyTask.SectionTitle}");
                    readyTask.LastRunTime = DateTime.Now;

                    if (stopwatch != null)
                    {
                        stopwatch.Stop();

                        // Diagnostics are not allowed to end the session either: with debug off none of this
                        // runs, and the console is the one writing the table, not the bot.
                        try
                        {
                            Logger.Info($"[Task] {readyTask.SectionTitle} finished in {stopwatch.Elapsed.TotalSeconds:0.###}s | Next: {readyTask.NextRunTime:dd/MM/yyyy HH:mm:ss}");

                            // The table is already live on the status screen (F2). The blank line is part of it.
                            Console.WriteLine();
                            PrintTasksStatusTable();
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"[FAILED] Reporting {readyTask.SectionTitle} threw: " +
                                         $"{e.GetType().Name} - {e.Message}");
                        }
                    }

                    yield return RunSafe(Watchdog.ForceClearAll($"after {readyTask.SectionTitle}"),
                        $"Watchdog cleanup after {readyTask.SectionTitle}");
                }
                finally
                {
                    IsTaskExecuting = false;
                    _executingTask = null;
                }
            }

            // The wait is scaled time, so this is the one moment where a stopped game clock shows. Armed
            // around the wait alone: WatchForStall reads it as "asleep past its due", and while the loop is
            // working there is no sleep to be late for.
            _nextScanDueAt = DateTime.Now + TimeSpan.FromSeconds(ScanInterval);
            yield return new WaitForSeconds(ScanInterval);
        }
    }

    private static IEnumerator RunSafe(IEnumerator routine, string context)
    {
        if (routine == null)
        {
            Logger.Info($"[FAILED] {context} returned null routine.");
            yield break;
        }

        var timeoutSeconds = MaxTaskRuntime;
        var timeoutEnabled = timeoutSeconds > 0f;
        var stopwatch = Stopwatch.StartNew();

        while (true)
        {
            object current = null;
            bool movedNext;

            if (timeoutEnabled && stopwatch.Elapsed.TotalSeconds > timeoutSeconds)
            {
                Logger.Info($"[FAILED] {context} timed out after {timeoutSeconds:0.###}s.");
                yield break;
            }

            try
            {
                movedNext = routine.MoveNext();
                if (movedNext) current = routine.Current;
            }
            catch (Exception e)
            {
                Logger.Info($"[FAILED] {context} threw: {e.GetType().Name} - {e.Message}");
                yield break;
            }

            if (!movedNext) yield break;

            // The routine produced a step, so the execution as a whole is alive: WatchForStall reads this.
            // It is the only signal that survives a step which never returns — the timeout above is checked
            // between steps, so that step is exactly the one it cannot see.
            _lastStepAt = DateTime.Now;

            yield return current;
        }
    }

    /// <summary>
    ///     Prints the status table to the console.
    ///     It must only be called when <see cref="Logger.IsDebugEnabled" /> is true: the lines use
    ///     <c>Info</c> on purpose, to preserve the box alignment instead of prefixing 13 lines with
    ///     [DEBUG]. The caller is what guarantees the gate.
    /// </summary>
    private static void PrintTasksStatusTable()
    {
        var now = DateTime.Now;
        Logger.Info($"[Bot Status] Task Table - {now:dd/MM/yyyy HH:mm:ss}");

        // Same column order as the in-game screen, so a log line and a screenshot can be read side by side.
        Logger.Info("| Task                      | Status        | Time Left   | Next Run            | Last Run            |");
        Logger.Info("|---------------------------|---------------|-------------|---------------------|---------------------|");

        // The rows come from the same place the in-game screen consumes; only the console layout is left here.
        foreach (var row in GetStatusRows(now))
            Logger.Info(
                $"| {row.Name,-25} | {row.Status,-13} | {row.TimeLeft,-11} | {FormatConsoleDate(row.NextRun),-19} | {FormatConsoleDate(row.LastRun),-19} |");
    }

    /// <summary>Day first, the same convention the status screen prints.</summary>
    private static string FormatConsoleDate(DateTime? value)
        => value?.ToString("dd/MM/yyyy HH:mm:ss") ?? TaskStatusRow.NoValue;

    /// <summary>
    ///     One row per task: the one being executed first, then the ones with a next run, and last the ones
    ///     that cannot run — disabled, or waiting for a level.
    ///     <paramref name="now" /> is a parameter rather than an internal DateTime.Now so the consumer can
    ///     line up "Time Left" with its own timestamp, instead of each one taking a slightly different instant.
    /// </summary>
    internal static List<TaskStatusRow> GetStatusRows(DateTime now)
    {
        var rows = new List<TaskStatusRow>(Tasks.Count);
        AppendStatusRows(rows, now);
        return rows;
    }

    /// <summary>
    ///     Fills an existing buffer, so consumers that refresh every second (the status screen) do not
    ///     allocate a new list on every refresh.
    /// </summary>
    internal static void AppendStatusRows(List<TaskStatusRow> buffer, DateTime now)
    {
        // Three groups: the task being executed (what the user is watching), the tasks that have a next run,
        // and the ones that cannot run — disabled, or locked until a level. The last group shows nothing but
        // "-", and its NextRunTime is MinValue in every row, so ordering by next run alone put exactly the
        // rows with nothing to say at the top of the table. Name breaks the tie inside a group: MinValue ties
        // would otherwise be decided by the order the tasks happen to be loaded in.
        foreach (var t in Tasks
                     .OrderBy(t => t == _executingTask ? 0 : HasPendingRun(t) ? 1 : 2)
                     .ThenBy(t => t.NextRunTime)
                     .ThenBy(t => t.SectionTitle))
        {
            var hasPendingRun = HasPendingRun(t);

            buffer.Add(new TaskStatusRow(
                t.SectionTitle,
                GetTaskStatus(t),
                // A task without a next run has no time left either. Without this the subtraction runs anyway
                // — from DateTime.MinValue for a task that never ran — and the column shows "0s" beside an
                // empty Next Run.
                hasPendingRun ? TimeParser.FormatFriendlyDuration(t.NextRunTime - now) : TaskStatusRow.NoValue,
                hasPendingRun ? t.NextRunTime : (DateTime?)null,
                t.LastRunTime));
        }
    }

    /// <summary>
    ///     Whether the task has a next run worth showing. Disabled is the user's choice and a level lock is the
    ///     game's; in neither case can the task run now, so neither has a next run or a time left.
    /// </summary>
    private static bool HasPendingRun(BotTask t) => t.IsEnabled && !t.IsLevelLocked;

    private static string GetTaskStatus(BotTask t)
    {
        if (!t.IsEnabled) return TaskStatusRow.Disabled;
        if (t.IsLevelLocked) return TaskStatusRow.LevelLocked;
        if (t.IsNotificationVisible()) return TaskStatusRow.Popup;
        return t.IsReady() ? TaskStatusRow.Ready : TaskStatusRow.Waiting;
    }
}