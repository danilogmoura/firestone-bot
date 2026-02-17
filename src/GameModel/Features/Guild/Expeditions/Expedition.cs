using System;
using System.Collections;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Guild.Expeditions;

public static class Expedition
{
    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.GuildLoc.ExpeditionLoc.CloseBtn).Click();

    public static bool IsExpeditionActive =>
        new GameElement(Paths.MenusLoc.CanvasLoc.GuildLoc.ExpeditionLoc.ActiveExpedition).IsVisible();

    public static DateTime NextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.GuildLoc.ExpeditionLoc.NextRunTimeTxt).Time;

    public static IEnumerator Claim =>
        new GameButton(Paths.MenusLoc.CanvasLoc.GuildLoc.ExpeditionLoc.ClaimBtn).Click();

    public static DateTime CurrentRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.GuildLoc.ExpeditionLoc.CurrentRunTimeTxt).Time;

    public static IEnumerator Start =>
        new GameButton(Paths.MenusLoc.CanvasLoc.GuildLoc.ExpeditionLoc.StartBtn).Click();
}