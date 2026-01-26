using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Firebot.Utils;
using MelonLoader;

namespace Firebot.Bot.Automation.Core;

public static class AutomationHandler
{
    private static bool _isProcessing;

    public static List<AutomationObserver> Observers { get; private set; } = new();

    public static void AutoRegister(string configFilePath)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var handlerNamespace = typeof(AutomationHandler).Namespace ?? string.Empty;
        var targetNamespace = handlerNamespace.EndsWith(".Core", StringComparison.Ordinal)
            ? handlerNamespace.Substring(0, handlerNamespace.Length - ".Core".Length)
            : handlerNamespace;

        const string suffix = "Automation";

        var automationTypes = assembly.GetTypes()
            .Where(t => typeof(AutomationObserver).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract
                        && t.Namespace != null && t.Namespace.StartsWith(targetNamespace) && t.Name.EndsWith(suffix))
            .ToList();

        foreach (var type in automationTypes)
            try
            {
                var instance = (AutomationObserver)Activator.CreateInstance(type);

                if (instance != null)
                {
                    instance.InitializeConfig(configFilePath);
                    Observers.Add(instance);
                }

                LogManager.Debug(nameof(AutomationHandler), $"[AutoRegister] Loaded & Configured: {type.Name}");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to load {type.Name}: {ex.Message}");
            }

        Observers = Observers.OrderBy(o => o.Priority).ToList();
        MelonLogger.Msg($"[AutoRegister] {Observers.Count} automations loaded and ordered.");
    }

    public static void CheckNotifications()
    {
        if (_isProcessing) return;

        foreach (var observer in Observers.Where(observer => observer.IsEnabled && observer.ShouldExecute()))
        {
            MelonCoroutines.Start(ExecuteRoutine(observer));
            break;
        }
    }

    private static IEnumerator ExecuteRoutine(AutomationObserver observer)
    {
        _isProcessing = true;
        yield return observer.OnNotificationTriggered();
        _isProcessing = false;
    }

    public static T GetObserver<T>() where T : AutomationObserver => Observers.OfType<T>().FirstOrDefault();
}