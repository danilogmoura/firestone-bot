using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Engineer.Tools;

public static class GaragePopup
{
    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.EngineerLoc.GarageLoc.Close).Click();

    public static IEnumerator ClickEngineer =>
        new GameButton(Paths.MenusLoc.CanvasLoc.EngineerLoc.GarageLoc.EngineerBtn).Click();
}