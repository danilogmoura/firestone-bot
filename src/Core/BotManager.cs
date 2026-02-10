using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Firebot.Behaviors;
using MelonLoader;
using UnityEngine;

namespace Firebot.Core;

public static class BotManager
{
    private static List<BotTask> _tasks = new();
    private static object _botRoutineHandle;
    public static bool IsRunning { get; private set; }

    public static void Initialize()
    {
        var configPath = BotSettings.ConfigPath;
        const string targetNamespace = "Firebot.Behaviors.Tasks";

        _tasks.Clear();

        var assembly = Assembly.GetExecutingAssembly();
        var taskTypes = assembly.GetTypes()
            .Where(t => t.Namespace != null && t.Namespace.StartsWith(targetNamespace) &&
                        t.IsSubclassOf(typeof(BotTask)) && !t.IsAbstract)
            .ToList();

        foreach (var type in taskTypes)
            try
            {
                var task = (BotTask)Activator.CreateInstance(type);
                task.InitializeConfig(configPath);
                _tasks.Add(task);
                Logger.Debug($"[Loader] Registered task: {task.Name} (section: {task.SectionTitle})");
            }
            catch (Exception e)
            {
                Logger.Debug($"[Loader] Failed to load {type.Name}: {e.GetType().Name} - {e.Message}");
            }

        _tasks = _tasks.OrderBy(t => t.Priority).ToList();
    }

    public static void Start()
    {
        if (IsRunning) return;
        if (_tasks.Count == 0) Initialize();

        IsRunning = true;
        _botRoutineHandle = MelonCoroutines.Start(BotSchedulerLoop());
        Logger.Debug($"[Firebot] Started. Tasks loaded: {_tasks.Count}");
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
        var startDelay = BotSettings.StartBotDelay;
        yield return new WaitForSeconds(startDelay);

        Logger.Info("Executando limpeza inicial de Popups...");
        yield return Watchdog.ForceClearAll();

        while (IsRunning)
        {
            var readyTask = _tasks.Where(t => t.IsReady())
                .OrderBy(t => t.Priority)
                .ThenBy(t => t.NextRunTime)
                .FirstOrDefault();

            if (readyTask != null)
            {
                var sw = Stopwatch.StartNew();
                Logger.Info(
                    $"[TASK START] Starting task: {readyTask.Name} (section: {readyTask.SectionTitle}, priority: {readyTask.Priority}, nextRun: {readyTask.NextRunTime:O})");

                yield return readyTask.Execute();

                sw.Stop();
                Logger.Info(
                    $"[TASK END] {readyTask.Name} (section: {readyTask.SectionTitle}) finished in {sw.Elapsed.TotalSeconds:F2}s");
            }

            yield return new WaitForSeconds(1f);
        }
    }
}