using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Map.Missions;

public static class MissionRewardsPopup
{
    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapLoc.MissionsLoc.MissionRewardsLoc.CloseBtn).Click();
}