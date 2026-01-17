using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader;

namespace FireBot.Bot.Automation.Core
{
    public static class AutomationHandler
    {
        private static readonly List<IAutomationObserver> Observers = new List<IAutomationObserver>();
        private static bool _isProcessing;

        public static void AutoRegister()
        {
            var assembly = Assembly.GetExecutingAssembly();

            const string targetNamespace = "FireBot.Bot.Automation";
            const string suffix = "Automation";

            var automationTypes = assembly.GetTypes()
                .Where(t => typeof(IAutomationObserver).IsAssignableFrom(t)
                            && !t.IsInterface
                            && !t.IsAbstract
                            && t.Namespace != null && t.Namespace.StartsWith(targetNamespace)
                            && t.Name.EndsWith(suffix))
                .ToList();

            foreach (var type in automationTypes)
                try
                {
                    var instance = (IAutomationObserver)Activator.CreateInstance(type);
                    Observers.Add(instance);
                    MelonLogger.Msg($"[AutoRegister] Carregado: {type.Name} do namespace {type.Namespace}");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Falha ao carregar {type.Name}: {ex.Message}");
                }
        }

        public static void CheckNotifications()
        {
            if (_isProcessing) return;

            foreach (var observer in Observers.Where(observer => observer.ToogleCondition()))
            {
                MelonCoroutines.Start(ExecuteRoutine(observer));
                break;
            }
        }

        private static IEnumerator ExecuteRoutine(IAutomationObserver observer)
        {
            _isProcessing = true;
            yield return observer.OnNotificationTriggered();
            _isProcessing = false;
        }
    }
}