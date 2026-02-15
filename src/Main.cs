using System.Reflection;
using Firebot.Core;
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
    private bool _isGameReady;

    public override void OnInitializeMelon()
    {
        BotSettings.Initialize();
        BotManager.Initialize();

        LoggerInstance.Msg("Firebot System Initialized.");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        _isGameReady = sceneName == "mainScene" || sceneName == "Game";

        if (_isGameReady)
        {
            if (BotSettings.AutoStart) BotManager.Start();
        }
        else
            BotManager.Stop();
    }

    public override void OnUpdate()
    {
        if (!_isGameReady || !Input.GetKeyDown(BotSettings.ShortcutKey)) return;

        if (BotManager.IsRunning)
            BotManager.Stop();
        else
            BotManager.Start();
    }
}