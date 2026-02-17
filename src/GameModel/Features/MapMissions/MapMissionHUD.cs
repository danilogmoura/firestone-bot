using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.MapMissions;

public class MapMissionHUD
{
    public static GameText MissionRefresh => new(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.HUDLoc.Refresh);
    public static GameButton CloseButton => new(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.HUDLoc.Close);
    public static GameButton WarfrontCampaignButton => new(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.HUDLoc.WarfrontBtn);
}