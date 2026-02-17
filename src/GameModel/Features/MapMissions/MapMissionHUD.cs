using System;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.MapMissions;

[Obsolete("This class is deprecated. Will be removed in a future update.")]
public class MapMissionHUD
{
    public static GameText MissionRefresh => new(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.HUDLoc.Refresh);
    public static GameButton CloseButton => new(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.HUDLoc.Close);
    public static GameButton WarfrontCampaignButton => new(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.HUDLoc.WarfrontBtn);
}