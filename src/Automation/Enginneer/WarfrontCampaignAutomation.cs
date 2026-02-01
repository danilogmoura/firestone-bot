using System.Collections;
using Firebot.Automation.Core;
using Firebot.GameModel.TMProComponents;
using Firebot.GameModel.Wrappers;
using static Firebot.Core.Paths.WarfrontCampaign;

namespace Firebot.Automation.Enginneer;

public class WarfrontCampaignAutomation : AutomationObserver
{
    public override bool ShouldExecute() => base.ShouldExecute() && Button.Notification.IsActive();

    public override IEnumerator OnNotificationTriggered()
    {
        if (!Button.Notification.IsActive()) yield break;

        yield return Button.Notification.Click();

        ResetSchedule();

        var claimToolsButton = new ButtonWrapper(ClaimToolsButton);
        var closeButton = new ButtonWrapper(CloseButton);

        if (claimToolsButton.IsInteractable()) yield return claimToolsButton.Click();

        var timer = new TextDisplay(NextLootTimeLeft);
        ScheduleNextCheck(timer.TotalSeconds);

        yield return closeButton.Click();
    }

    private static class Button
    {
        public static readonly ButtonWrapper Notification = new(WarfrontCampaignNotification);
    }
}