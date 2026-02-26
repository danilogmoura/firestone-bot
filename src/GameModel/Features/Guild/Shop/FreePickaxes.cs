using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using Firebot.Utilities;

namespace Firebot.GameModel.Features.Guild.Shop;

public static class FreePickaxes
{
    public static DateTime NextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.GuildLoc.GuildShopLoc.FreePickaxeLoc.NextRunTimeTxt).Time;

    private static string QuantityTxt =>
        new GameText(Paths.MenusLoc.CanvasLoc.GuildLoc.GuildShopLoc.FreePickaxeLoc.QuantityTxt).GetParsedText();

    public static int Quantity => StringUtils.TryParseIntFromString(QuantityTxt, out var val) ? val : 0;

    public static IEnumerator Claim =>
        new GameButton(Paths.MenusLoc.CanvasLoc.GuildLoc.GuildShopLoc.FreePickaxeLoc.ClaimBtn).Click();
}