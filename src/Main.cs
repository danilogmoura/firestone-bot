using System.Reflection;
using FireBot.Bot.Automation.Core;
using FireBot.Core;
using FireBot.Utils;
using MelonLoader;
using UnityEngine;
using Main = FireBot.Main;

[assembly: MelonInfo(typeof(Main), "FireBot", "0.1.0", "danilogmoura", "https://github.com/danilogmoura/fire-bot")]
[assembly: MelonGame]

[assembly: MelonColor(255, 255, 0, 255)]
[assembly: MelonAuthorColor(255, 0, 255, 0)]

[assembly: AssemblyTitle("FireBot")]
[assembly:
    AssemblyDescription("A bot for automating tasks using MelonLoader.")]
[assembly: AssemblyCopyright("Created by danilogmoura")]

namespace FireBot;

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