using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.MapMissions;

public static class MapMission
{
    public static DateTime MissionRefresh => new GameText(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.HUDLoc.Refresh).Time;

    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.HUDLoc.Close).Click();
}