using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town.Engineer;

public static class Engineer
{
    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.EngineerLoc.CloseBtn).Click();

    public static IEnumerator Claim =>
        new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.EngineerLoc.ClaimBtn).Click();

    public static DateTime NextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.EngineerLoc.NextRunTimeTxt).Time;
}