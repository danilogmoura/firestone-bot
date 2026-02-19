using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town.Library.FirestoneResearch;

public static class Preview
{
    public static bool IsUnlocked =>
        new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.PreviewLoc.UnlockedTxt).IsVisible();

    public static bool IsMaxed =>
        new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.PreviewLoc.MaxedTxt).IsVisible();

    public static IEnumerator Start =>
        new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.PreviewLoc.ActivateBtn).Click();
}