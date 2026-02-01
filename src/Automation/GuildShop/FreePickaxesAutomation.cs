using System.Collections;
using Firebot.Automation.Core;
using Firebot.GameModel.Wrappers;
using static Firebot.Core.Paths.GuildShop;

namespace Firebot.Automation.GuildShop;

public class FreePickaxesAutomation : AutomationObserver
{
    public override int Priority => 25;

    public override bool ShouldExecute() => base.ShouldExecute() && Button.Notification.IsActive();

    public override IEnumerator OnNotificationTriggered()
    {
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