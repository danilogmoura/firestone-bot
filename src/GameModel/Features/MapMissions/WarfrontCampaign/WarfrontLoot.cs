using System;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.MapMissions.WarfrontCampaign;

public static class WarfrontLoot
{
    public static GameButton ClaimToolsButton => new(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.WarfrontLoc.Claim);

    public static DateTime FindNextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.WarfrontLoc.NextLoot).Time;
}