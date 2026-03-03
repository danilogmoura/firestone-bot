using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.MainScene;

public static class Upgrades
{
    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.CloseBtn).Click();
}