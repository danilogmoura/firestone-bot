using System;
using Firebot.Core;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Features.MapMissions.WarfrontCampaign;

public static class WarfrontLoot
{
    public static GameButton ClaimToolsButton => new(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.WarfrontLoc.Claim);

    public static DateTime FindNextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.WarfrontLoc.NextLoot).Time;
}