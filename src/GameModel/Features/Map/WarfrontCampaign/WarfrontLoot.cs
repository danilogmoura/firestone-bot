using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Map.WarfrontCampaign;

public static class WarfrontLoot
{
    public static IEnumerator ClaimTools =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapLoc.WarfrontLoc.Claim).Click();

    public static DateTime NextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.MapLoc.WarfrontLoc.NextRunTime).Time;
}