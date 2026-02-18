using System;
using System.Collections;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town.MagicQuarters;

public static class MagicQuarters
{
    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.MagicQuarters.CloseBtn).Click();

    public static GameButton TrainBtn => new(Paths.MenusLoc.CanvasLoc.TownLoc.MagicQuarters.TrainBtn);

    public static DateTime NextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.MagicQuarters.NextRunTimeTxt).Time;

    public static IEnumerator CloseLockedPopup =>
        new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.MagicQuarters.LockedGuardianLoc.CloseBtn).Click();

    public static GameElement Guardians => new(Paths.MenusLoc.CanvasLoc.TownLoc.MagicQuarters.Guardoians);
}