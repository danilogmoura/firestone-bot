using System.Collections;
using Firebot.Bot.Automation.Core;
using Firebot.Bot.Component;
using Firebot.Utils;
using static Firebot.Utils.Paths.GuildShop;

namespace Firebot.Bot.Automation.GuildShop;

public class FreePickaxesAutomation : AutomationObserver
{
    public override string SectionName => "Free Pickaxes";
    public override int Priority => 25;

    public override bool ToogleCondition()
    {
        return Button.Notification.IsActive();
    }

    public override IEnumerator OnNotificationTriggered()
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
        public static readonly ButtonWrapper Notification = new(FreePickaxesNotificationPath);

        public static readonly ButtonWrapper Close = new(CloseButtonPath);

        public static readonly ButtonWrapper FreePickaxeItem = new(FreePickaxeItemPath);
    }
}