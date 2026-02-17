using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Engineer.Tools;

public static class EngineerSubmenu
{
    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.EngineerLoc.SubmenuLoc.Close).Click();

    public static IEnumerator ClaimTools =>
        new GameButton(Paths.MenusLoc.CanvasLoc.EngineerLoc.SubmenuLoc.ClaimTools).Click();

    public static DateTime FindNextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.EngineerLoc.SubmenuLoc.Cooldown).Time;
}