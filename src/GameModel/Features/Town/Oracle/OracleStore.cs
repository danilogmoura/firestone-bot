using System;
using System.Collections;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town.Oracle;

public static class OracleStore
{
    private static GameElement OracleStoreGameElement => new(Paths.MenusLoc.CanvasLoc.OracleStoreLoc.Root);

    public static IEnumerator Close => new GameButton(Paths.MenusLoc.CanvasLoc.OracleStoreLoc.CloseBtn).Click();

    public static IEnumerator ClaimGift => new GameButton(Paths.MenusLoc.CanvasLoc.OracleStoreLoc.OraclesGift).Click();

    public static DateTime NextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.OracleStoreLoc.OraclesGiftRenewTxt).Time;
}