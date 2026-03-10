using System.Collections;
using System.Collections.Generic;
using Firebot.Core;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Map.LiberationMissions;

public static class LiberationMissionsPopup
{
    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapLoc.WarfrontLoc.DailyMissionsLoc.LiberationMissionsPopup.CloseBtn)
            .Click();

    private static IEnumerable<GameButton> Missions()
    {
        var gameElement = new GameElement(Paths.MenusLoc.CanvasLoc.MapLoc.WarfrontLoc.DailyMissionsLoc
            .LiberationMissionsPopup.Missions);
        var missions = gameElement.GetChildren();

        Logger.Debug("[LiberationMissionsPopup] Scanning liberation missions.");
        var index = 0;

        foreach (var mission in missions)
        {
            index++;
            var isLocked =
                new GameElement(Paths.MenusLoc.CanvasLoc.MapLoc.WarfrontLoc.DailyMissionsLoc.LiberationMissionsPopup
                    .Locked).IsVisible();

            var isActive = new GameElement(Paths.MenusLoc.CanvasLoc.MapLoc.WarfrontLoc.DailyMissionsLoc
                .LiberationMissionsPopup.FightBtn).IsVisible();

            var isWon = new GameElement(Paths.MenusLoc.CanvasLoc.MapLoc.WarfrontLoc.DailyMissionsLoc
                .LiberationMissionsPopup.WonTxt).IsVisible();

            if (isLocked || !isActive || isWon)
            {
                Logger.Debug($"[LiberationMissionsPopup] Mission {index} skipped (locked={isLocked}, active={isActive}, won={isWon}).");
                continue;
            }

            Logger.Debug($"[LiberationMissionsPopup] Mission {index} ready to start.");

            yield return new GameButton(
                Paths.MenusLoc.CanvasLoc.MapLoc.WarfrontLoc.DailyMissionsLoc.LiberationMissionsPopup.FightBtn, mission);
        }
    }

    public static IEnumerator StartMission()
    {
        var started = 0;

        foreach (var mission in Missions())
        {
            started++;
            Logger.Info($"[LiberationMissionsPopup] Starting liberation mission #{started}.");
            yield return mission.Click();

            if (!Fight()) Logger.Warning($"[LiberationMissionsPopup] Fight routine returned false for mission #{started}.");
        }

        if (started == 0)
            Logger.Info("[LiberationMissionsPopup] No available liberation missions to start.");
        else
            Logger.Info($"[LiberationMissionsPopup] Started {started} liberation mission(s).");
    }

    public static bool Fight()
    {
        return true;
    }
}