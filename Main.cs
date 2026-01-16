using System;
using System.Collections;
using FireBot.Bot.Automation.Enginneer;
using FireBot.Bot.Automation.Expedition;
using FireBot.Bot.Automation.Library;
using FireBot.Bot.Automation.Main;
using FireBot.Bot.Automation.Mission;
using FireBot.Bot.Automation.Oracle;
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
        private const double MissionLogIntervalSeconds = 60;

        private DateTime _nextExecutionTime;

        private bool _showMenu;

        public override void OnLateInitializeMelon()
        {
            LogManager.Initialize(LoggerInstance);
            _nextExecutionTime = DateTime.Now.AddSeconds(MissionLogIntervalSeconds);
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F1)) _showMenu = !_showMenu;

            if (DateTime.Now >= _nextExecutionTime)
            {
                _nextExecutionTime = DateTime.Now.AddSeconds(MissionLogIntervalSeconds);
                MelonCoroutines.Start(RunAllSequentially());
            }
        }

        private static IEnumerator RunAllSequentially()
        {
            LogManager.Header($"Starting automations - {DateTime.Now:HH:mm:ss}");

            yield return OfflineProgressAutomation.Process();
            yield return ToolsProductionAutomation.Process();
            yield return WarfrontCampaignAtomation.Process();
            yield return MissionMapAutomation.Process();
            yield return ExpeditionAutomation.Process();
            yield return FirestoneResearchAutomation.Process();
            yield return OracleRitualsAutomation.Process();

            LogManager.WriteLine();
        }
    }
}