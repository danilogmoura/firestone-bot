using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.MapMissions.WarfrontCampaign;

public static class WarfrontLoot
{
    public static IEnumerator ClaimTools =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.WarfrontLoc.Claim).Click();

    public static DateTime FindNextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.WarfrontLoc.NextLoot).Time;
}