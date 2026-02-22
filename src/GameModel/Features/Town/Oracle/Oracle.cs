using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town.Oracle;

public static class Oracle
{
    public static IEnumerator Close => new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.OracleLoc.CloseBtn).Click();
}