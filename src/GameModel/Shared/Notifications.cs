using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Shared;

public static class Notifications
{
    public static IEnumerator Engineer =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.Engineer).Click();

    public static IEnumerator WarfrontCampaign =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.WarfrontCampaign).Click();

    public static IEnumerator FreePickaxes =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.FreePickaxes).Click();

    public static IEnumerator Expeditions =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.Expeditions).Click();

    public static IEnumerator Quests =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.Quests).Click();

    public static IEnumerator GuardianTraining =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.GuardianTraining).Click();

    public static IEnumerator FirestoneResearch =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.FirestoneResearch).Click();

    public static IEnumerator Experiments =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.Experiments).Click();

    public static IEnumerator OracleRituals =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.OracleRituals).Click();

    public static IEnumerator MapMissions =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.MapMissions).Click();
}