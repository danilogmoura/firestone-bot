using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Firebot.Core.Tasks;
using MelonLoader;
using UnityEngine;
using static Firebot.Core.BotSettings;

namespace Firebot.Core;

public static class BotManager
{
    private static List<BotTask> _tasks = new();
    private static object _botRoutineHandle;
    public static bool IsRunning { get; private set; }

    public static void Initialize()
    {
        var configPath = ConfigPath;
        const string targetNamespace = "Firebot.Behaviors";

        _tasks.Clear();

        var assembly = Assembly.GetExecutingAssembly();
        var taskTypes = assembly.GetTypes()
            .Where(task => task.Namespace != null && task.Namespace.StartsWith(targetNamespace) &&
                           task.IsSubclassOf(typeof(BotTask)) && !task.IsAbstract)
            .ToList();

        foreach (var type in taskTypes)
            try
            {
                var task = (BotTask)Activator.CreateInstance(type);
                if (task != null)
                {
                    task.InitializeConfig(configPath);
                    _tasks.Add(task);
                    Logger.Debug($"[Loader] Registered task: {task.SectionTitle}");
                }
            }
            catch (Exception e)
            {
                Logger.Debug($"[Loader] Failed to load {type.Name}: {e.GetType().Name} - {e.Message}");
            }

        _tasks = _tasks.ToList();
    }

    public static void Start()
    {
        if (IsRunning) return;
        if (_tasks.Count == 0) Initialize();

        IsRunning = true;
        _botRoutineHandle = MelonCoroutines.Start(BotSchedulerLoop());
        Logger.Debug($"Started. Tasks loaded: {_tasks.Count}");
    }

    public static void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
        if (_botRoutineHandle != null) MelonCoroutines.Stop(_botRoutineHandle);
        Logger.Debug("Stopped.");
    }

    private static IEnumerator BotSchedulerLoop()
    {
        if (AutoStart) yield return new WaitForSeconds(StartBotDelay);

        while (IsRunning)
        {
            BotTask readyTask = null;
            var earliest = DateTime.MaxValue;

            foreach (var task in _tasks)
            {
                if (!task.IsReady()) continue;
                if (task.NextRunTime >= earliest) continue;

                earliest = task.NextRunTime;
                readyTask = task;
            }

            if (readyTask != null)
            {
                yield return RunSafe(Watchdog.ForceClearAll(), $"Watchdog cleanup before {readyTask.SectionTitle}");

                var stopwatch = Stopwatch.StartNew();
                Logger.Info(
                    $"[Task] Started: {readyTask.SectionTitle} (NextRunTime: {readyTask.NextRunTime:MM/dd/yyyy HH:mm:ss})");

                yield return RunSafe(readyTask.Execute(), $"Task {readyTask.SectionTitle}");

                stopwatch.Stop();
                Logger.Info(
                    $"[Task] Finished: {readyTask.SectionTitle} in {stopwatch.Elapsed.TotalSeconds:0.###}s (NextRunTime: {readyTask.NextRunTime:MM/dd/yyyy HH:mm:ss})");

                yield return RunSafe(Watchdog.ForceClearAll(), $"Watchdog cleanup after {readyTask.SectionTitle}");
            }

            yield return new WaitForSeconds(ScanInterval);
        }
    }

    private static IEnumerator RunSafe(IEnumerator routine, string context)
    {
        if (routine == null)
        {
            Logger.Debug($"[FAILED] {context} returned null routine.");
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
                Logger.Debug($"[FAILED] {context} timed out after {timeoutSeconds:0.###}s.");
                yield break;
            }

            try
            {
                movedNext = routine.MoveNext();
                if (movedNext) current = routine.Current;
            }
            catch (Exception e)
            {
                Logger.Debug($"[FAILED] {context} threw: {e.GetType().Name} - {e.Message}");
                yield break;
            }

            if (!movedNext) yield break;
            yield return current;
        }
    }
}