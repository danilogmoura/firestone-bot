using System.Collections;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Utils.Paths.WarfrontCampaign;

namespace FireBot.Bot.Automation.Enginneer
{
    public class WarfrontCampaignAutomation : AutomationObserver
    {
        public override bool ToogleCondition()
        {
            return Button.Notification.IsActive();
        }

        public override IEnumerator OnNotificationTriggered()
        {
            if (!Button.Notification.IsActive()) yield break;

            LogManager.SubHeader("Warfront Campaign Scrolls");
            yield return Button.Notification.Click();

            var claimToolsButton = new ButtonWrapper(ClaimToolsButton);
            var closeButton = new ButtonWrapper(CloseButton);

            if (claimToolsButton.IsInteractable()) yield return claimToolsButton.Click();

            yield return closeButton.Click();
        }

        private static class Button
        {
            public static readonly ButtonWrapper Notification = new ButtonWrapper(WarfrontCampaignNotification);
        }
    }
}