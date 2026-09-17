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
    private static readonly List<BotTask> Tasks = new();
    private static object _botRoutineHandle;
    private static bool _shouldPauseAutoUpgrade;
    public static bool IsRunning { get; private set; }
    private static bool IsTaskExecuting { get; set; }

    public static bool ShouldPauseAutoUpgrade() => IsRunning && (_shouldPauseAutoUpgrade || IsTaskExecuting);

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
        _botRoutineHandle = MelonCoroutines.Start(BotSchedulerLoop());
        Logger.Info($"Started. Tasks loaded: {Tasks.Count(t => t.IsEnabled)}");
    }

    public static void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
        IsTaskExecuting = false;
        _shouldPauseAutoUpgrade = false;
        if (_botRoutineHandle != null) MelonCoroutines.Stop(_botRoutineHandle);
        Logger.Info("Stopped.");
    }

    private static IEnumerator BotSchedulerLoop()
    {
        if (AutoStart) yield return new WaitForSeconds(StartBotDelay);

        while (IsRunning)
        {
            BotTask notificationTask = null;
            BotTask readyTask = null;
            var earliest = DateTime.MaxValue;
            var nextEnabledTaskRun = DateTime.MaxValue;

            foreach (var task in Tasks)
            {
                if (task.IsEnabled && task.NextRunTime < nextEnabledTaskRun)
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
                              (nextEnabledTaskRun - DateTime.Now).TotalSeconds <= AutoUpgradeMinExecutionWindowSeconds;
            _shouldPauseAutoUpgrade = notificationTask != null || readyTask != null || hasNearTask;

            if (readyTask != null)
            {
                IsTaskExecuting = true;
                try
                {
                    yield return RunSafe(Watchdog.ForceClearAll(), $"Watchdog cleanup before {readyTask.SectionTitle}");

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

                        Logger.Info($"[Task] {readyTask.SectionTitle} finished in {stopwatch.Elapsed.TotalSeconds:0.###}s | Next: {readyTask.NextRunTime:dd/MM/yyyy HH:mm:ss}");

                        // The table is already live on the status screen (F2). The blank line is part of it.
                        Console.WriteLine();
                        PrintTasksStatusTable();
                    }

                    yield return RunSafe(Watchdog.ForceClearAll(), $"Watchdog cleanup after {readyTask.SectionTitle}");
                }
                finally
                {
                    IsTaskExecuting = false;
                }
            }

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
    ///     One row per task, ordered by next run.
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
        foreach (var t in Tasks.OrderBy(t => t.NextRunTime))
            buffer.Add(new TaskStatusRow(
                t.SectionTitle,
                GetTaskStatus(t),
                // A disabled task has no next run that matters, so it has no time left either. Without this
                // the subtraction runs anyway — from DateTime.MinValue for a task that never ran — and the
                // column shows "0s" beside an empty Next Run.
                t.IsEnabled ? TimeParser.FormatFriendlyDuration(t.NextRunTime - now) : TaskStatusRow.NoValue,
                t.IsEnabled ? t.NextRunTime : (DateTime?)null,
                t.LastRunTime));
    }

    private static string GetTaskStatus(BotTask t)
    {
        if (!t.IsEnabled) return TaskStatusRow.Disabled;
        if (t.IsNotificationVisible()) return TaskStatusRow.Popup;
        return t.IsReady() ? TaskStatusRow.Ready : TaskStatusRow.Waiting;
    }
}