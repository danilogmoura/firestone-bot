using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town.Library;

public static class Library
{
    public static IEnumerator Close = new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.CloseBtn).Click();
}