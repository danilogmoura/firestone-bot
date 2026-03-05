using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Shared;

public static class Store
{
    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.StoreLoc.CloseBtn).Click();

    public static IEnumerator ClaimCheckIn =>
        new GameButton(Paths.MenusLoc.CanvasLoc.StoreLoc.CheckInLoc.CheckInBtn).Click();

    public static DateTime CheckInNextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.StoreLoc.CheckInLoc.NextRunTimeTxt).Time;

    public static IEnumerator ClaimMysteryBox =>
        new GameButton(Paths.MenusLoc.CanvasLoc.StoreLoc.MysteryBoxLoc.MysteryBoxBtn).Click();

    public static DateTime MysteryBoxNextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.StoreLoc.MysteryBoxLoc.NextRunTimeTxt).Time;
}