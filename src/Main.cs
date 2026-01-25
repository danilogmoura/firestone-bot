using System.Reflection;
using Firebot.Bot.Automation.Core;
using Firebot.Core;
using Firebot.Utils;
using MelonLoader;
using UnityEngine;
using Main = Firebot.Main;

[assembly: MelonInfo(typeof(Main), "Firebot", "0.1.0", "danilogmoura", "https://github.com/danilogmoura/firebot")]
[assembly: MelonGame]

[assembly: MelonColor(255, 255, 0, 255)]
[assembly: MelonAuthorColor(255, 0, 255, 0)]

[assembly: AssemblyTitle("Firebot")]
[assembly:
    AssemblyDescription("A bot for automating tasks using MelonLoader.")]
[assembly: AssemblyCopyright("Created by danilogmoura")]

namespace Firebot;

public class Main : MelonMod
{
    private float _scanTimer;

    public override void OnInitializeMelon()
    {
        BotSettings.Initialize();
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