using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town.Engineer.Tools;

public static class EngineerSubmenu
{
    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.EngineerLoc.SubmenuLoc.Close).Click();

    public static IEnumerator ClaimTools =>
        new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.EngineerLoc.SubmenuLoc.ClaimTools).Click();

    public static DateTime NextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.EngineerLoc.SubmenuLoc.NextRunTime).Time;
}