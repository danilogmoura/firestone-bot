using System.Collections;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Utils.Paths.Engineer;

namespace FireBot.Bot.Automation.Enginneer
{
    public class ToolsProductionAutomation : AutomationObserver
    {
        public override bool ToogleCondition()
        {
            return Button.Notification.IsActive();
        }

        public override IEnumerator OnNotificationTriggered()
        {
            if (!Button.Notification.IsActive()) yield break;

            LogManager.SubHeader("Tools Production");
            yield return Button.Notification.Click();

            var caimToolsButton = new ButtonWrapper(ClaimToolsButton);

            if (caimToolsButton.IsInteractable()) yield return caimToolsButton.Click();

            var closeButton = new ButtonWrapper(CloseButton);
            yield return closeButton.Click();
        }

        private static class Button
        {
            public static readonly ButtonWrapper Notification = new ButtonWrapper(EngineerGridNotification);
        }
    }
}