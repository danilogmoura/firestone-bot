using System.Collections;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Shared;

public static class Notifications
{
    public static IEnumerator Engineer =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.EngineerBtn).Click();

    public static IEnumerator WarfrontCampaign =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.WarfrontCampaignBtn).Click();

    public static IEnumerator FreePickaxes =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.FreePickaxesBtn).Click();

    public static IEnumerator Expeditions =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.ExpeditionsBtn).Click();

    public static IEnumerator Quests =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.QuestsBtn).Click();

    public static IEnumerator GuardianTraining =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.GuardianTrainingBtn).Click();

    public static IEnumerator FirestoneResearch =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.FirestoneResearchBtn).Click();

    public static IEnumerator Experiments =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.ExperimentsBtn).Click();

    public static IEnumerator OracleRituals =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.OracleRitualsBtn).Click();

    public static IEnumerator MapMissions =>
        new GameNotificationButton(Paths.BattleLoc.NotificationsLoc.MapMissionsBtn).Click();
}