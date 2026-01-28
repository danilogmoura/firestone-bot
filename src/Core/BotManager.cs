using System;
using System.Collections;
using Firebot.Bot.Automation.Core;
using Firebot.Bot.Component.TextMeshPro;
using Firebot.Utils;
using MelonLoader;
using UnityEngine;
using Logger = Firebot.Utils.Logger;

namespace Firebot.Core;

public static class BotManager
{
    private static readonly Logger Log = new(nameof(BotManager));

    private static TextDisplay _statusColor;

    private static Coroutine _botLoop;

    public static bool IsRunning { get; private set; }

    public static void Start(float delay = 0f)
    {
        if (IsRunning) return;

        IsRunning = true;
        ToggleStatusColor();
        _botLoop = MelonCoroutines.Start(BotRoutine(delay)) as Coroutine;

        Log.Info(delay > 0
            ? $"Bot scheduled to start in {delay}s..."
            : "Bot started immediately!");
    }

    public static void Stop()
    {
        if (!IsRunning) return;

        IsRunning = false;
        ToggleStatusColor();
        if (_botLoop != null) MelonCoroutines.Stop(_botLoop);
        _botLoop = null;
        Log.Info("Bot stopped.");
    }

    private static IEnumerator BotRoutine(float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);

        while (IsRunning)
        {
            try
            {
                AutomationHandler.CheckNotifications();
            }
            catch (Exception ex)
            {
                Log.Error($"Execution error: {ex.Message}");
            }

            yield return new WaitForSeconds(BotSettings.ScanInterval);
        }
    }

    private static void ToggleStatusColor()
    {
        _statusColor ??= new TextDisplay(Paths.CharacterLevelPath);
        if (IsRunning) _statusColor.SetOutline(Color.green);
        else _statusColor.RemoveOutline();
    }
}