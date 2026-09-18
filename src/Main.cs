using System.Reflection;
using Firebot.BotActions;
using Firebot.Core;
using Firebot.UI;
using Firebot.UI.Widgets;
using MelonLoader;
using UnityEngine;
using Main = Firebot.Main;

[assembly: MelonInfo(typeof(Main), "Firebot", "0.3.0-alpha.1", "danilogmoura", "https://github.com/danilogmoura/firebot")]
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
        AutoSkill.Initialize();
        AutoUpgrade.Initialize();
        BotPanel.Initialize();
        BotStatusScreen.Initialize();

        LoggerInstance.Msg("Firebot System Initialized.");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        BotPanel.OnSceneChanged();
        BotStatusScreen.OnSceneChanged();

        _isGameReady = sceneName == "mainScene" || sceneName == "Game";

        if (_isGameReady)
        {
            if (BotSettings.AutoStart) BotManager.Start();
        }
        else BotManager.Stop();
    }

    public override void OnUpdate()
    {
        // Frozen once, before anything consumes it. A key press lasts the whole frame, so every GetKeyDown
        // below sees the same key: without this, recording F6 would also start AutoUpgrade and recording
        // F1 would also close the panel. It cannot be read later either — the panel consumes the capture,
        // so a reader placed after it would be told that nothing is being recorded and would steal the key.
        HotkeyGate.BeginFrame(KeybindRow.IsListening);

        if (_isGameReady && !HotkeyGate.IsCapturing && Input.GetKeyDown(BotSettings.ShortcutKey))
        {
            if (BotManager.IsRunning) BotManager.Stop();
            else BotManager.Start();
        }

        if (_isGameReady)
        {
            // Before the windows: a stalled task is stopped here, and the status screen then shows the bot as
            // stopped instead of as busy for one more refresh.
            BotManager.WatchForStall();

            BotPanel.Tick();
            BotStatusScreen.Tick();
        }
        else if (Input.GetKeyDown(BotSettings.PanelKey) || Input.GetKeyDown(BotStatusScreen.Key))
        {
            // Without this, pressing the hotkey while loading looks like the mod died: the key simply
            // does nothing and there is nothing in the log explaining why.
            LoggerInstance.Warning("[UI] Hotkey ignored: the game scene is not loaded yet.");
        }

        AutoSkill.Update();
        AutoUpgrade.Update();
    }
}