using System;
using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Guild.Shop;

public static class FreePickaxes
{
    public static IEnumerator Claim =>
        new GameButton(Paths.MenusLoc.CanvasLoc.GuildLoc.GuildShopLoc.FreePickaxeLoc.ClaimBtn).Click();

    public static DateTime NextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.GuildLoc.GuildShopLoc.FreePickaxeLoc.NextRunTimeTxt).Time;
}