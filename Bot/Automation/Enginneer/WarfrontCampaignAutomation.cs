using System.Collections;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Utils.Paths.WarfrontCampaign;

namespace FireBot.Bot.Automation.Enginneer
{
    public class WarfrontCampaignAutomation : IAutomationObserver
    {
        public bool ToogleCondition()
        {
            return Button.notification.IsActive();
        }

        public IEnumerator OnNotificationTriggered()
        {
            if (!Button.notification.IsActive()) yield break;

            LogManager.SubHeader("Warfront Campaign Scrolls");
            yield return Button.notification.Click();

            var claimToolsButton = new ButtonWrapper(ClaimToolsButton);
            var closeButton = new ButtonWrapper(CloseButton);

            if (claimToolsButton.IsInteractable()) yield return claimToolsButton.Click();

            yield return closeButton.Click();
        }

        private class Button
        {
            public static readonly ButtonWrapper notification = new ButtonWrapper(WarfrontCampaignNotification);
        }
    }
}