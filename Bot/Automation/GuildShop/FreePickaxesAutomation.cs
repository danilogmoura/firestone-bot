using System.Collections;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Utils.Paths.GuildShop;

namespace FireBot.Bot.Automation.GuildShop
{
    public class FreePickaxesAutomation : IAutomationObserver
    {
        public bool ToogleCondition()
        {
            return Button.Notification.IsActive();
        }

        public IEnumerator OnNotificationTriggered()
        {
            if (!Button.Notification.IsInteractable()) yield break;

            LogManager.SubHeader("Free Pickaxes");

            yield return Button.Notification.Click();

            if (Button.FreePickaxeItem.IsInteractable())
                yield return Button.FreePickaxeItem.Click();

            yield return Button.Close.Click();
        }

        private static class Button
        {
            public static readonly ButtonWrapper Notification = new ButtonWrapper(FreePickaxesNotificationPath);

            public static readonly ButtonWrapper Close = new ButtonWrapper(CloseButtonPath);

            public static readonly ButtonWrapper FreePickaxeItem = new ButtonWrapper(FreePickaxeItemPath);
        }
    }
}