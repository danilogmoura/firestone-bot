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
        private static List<AutomationObserver> _observers = new List<AutomationObserver>();
        private static bool _isProcessing;

        public static void AutoRegister()
        {
            var assembly = Assembly.GetExecutingAssembly();

            const string targetNamespace = "FireBot.Bot.Automation";
            const string suffix = "Automation";

            var automationTypes = assembly.GetTypes()
                .Where(t => typeof(AutomationObserver).IsAssignableFrom(t)
                            && !t.IsInterface
                            && !t.IsAbstract
                            && t.Namespace != null && t.Namespace.StartsWith(targetNamespace)
                            && t.Name.EndsWith(suffix))
                .ToList();

            foreach (var type in automationTypes)
                try
                {
                    var instance = (AutomationObserver)Activator.CreateInstance(type);
                    _observers.Add(instance);
                    MelonLogger.Msg($"[AutoRegister] Loaded: {type.Name} from namespace {type.Namespace}");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Failed to load{type.Name}: {ex.Message}");
                }

            _observers = _observers.OrderBy(o => o.Priority).ToList();
            MelonLogger.Msg($"[AutoRegister] {_observers.Count} automations loaded and ordered by priority.");
        }

        public static void CheckNotifications()
        {
            if (_isProcessing) return;

            foreach (var observer in _observers.Where(observer => observer.ToogleCondition()))
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
    }
}