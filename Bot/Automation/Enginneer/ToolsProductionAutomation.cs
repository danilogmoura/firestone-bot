using System.Collections;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Utils.Paths.Engineer;

namespace FireBot.Bot.Automation.Enginneer
{
    public class ToolsProductionAutomation : IAutomationObserver
    {
        public bool ToogleCondition()
        {
            return Button.notification.IsActive();
        }

        public IEnumerator OnNotificationTriggered()
        {
            if (!Button.notification.IsActive()) yield break;

            LogManager.SubHeader("Tools Production");
            yield return Button.notification.Click();

            var caimToolsButton = new ButtonWrapper(ClaimToolsButton);

            if (caimToolsButton.IsInteractable()) yield return caimToolsButton.Click();

            var closeButton = new ButtonWrapper(CloseButton);
            yield return closeButton.Click();
        }

        private class Button
        {
            public static readonly ButtonWrapper notification = new ButtonWrapper(EngineerGridNotification);
        }
    }
}