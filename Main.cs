using FireBot.Bot.Automation.Core;
using FireBot.Config;
using FireBot.Utils;
using MelonLoader;
using UnityEngine;

namespace FireBot
{
    public static class BuildInfo
    {
        public const string Name = "FireBot";
        public const string Description = "";
        public const string Author = "danilogmoura";
        public const string Company = null;
        public const string Version = "1.0.0";
    }

    public class Main : MelonMod
    {
        private float _scanTimer;

        public override void OnInitializeMelon()
        {
            BotSettings.Initialize();
            AutomationHandler.AutoRegister();
        }

        public override void OnLateInitializeMelon()
        {
            LogManager.Initialize(LoggerInstance);
        }

        public override void OnUpdate()
        {
            if (!BotSettings.IsBotEnabled.Value) return;

            _scanTimer -= Time.deltaTime;

            if (!(_scanTimer <= 0f)) return;

            _scanTimer = BotSettings.ScanInterval.Value;
            AutomationHandler.CheckNotifications();
        }
    }
}