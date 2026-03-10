using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Map.LiberationMissions;

public static class DailyMissionsPopup
{
    public static IEnumerator DailyMissions =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapLoc.WarfrontLoc.DailyMissionsBtn).Click();

    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapLoc.WarfrontLoc.DailyMissionsLoc.DailyMissionsPopup.CloseBtn)
            .Click();

    public static IEnumerator LiberationMissions =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapLoc.WarfrontLoc.DailyMissionsLoc.DailyMissionsPopup.OpenBtn)
            .Click();

    public static DateTime NextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.MapLoc.WarfrontLoc.DailyMissionsLoc.DailyMissionsPopup.NextRunTimeTxt)
            .Time;
}