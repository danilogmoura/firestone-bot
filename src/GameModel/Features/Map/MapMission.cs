using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Map;

public static class MapMission
{
    public static DateTime NextRunTime => new GameText(Paths.MenusLoc.CanvasLoc.MapLoc.NextRunTime).Time;

    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapLoc.Close).Click();
}