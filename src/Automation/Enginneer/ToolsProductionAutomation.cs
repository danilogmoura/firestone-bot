using System.Collections;
using Firebot.Automation.Core;
using Firebot.GameModel.Wrappers;
using static Firebot.Core.Paths.Engineer;

namespace Firebot.Automation.Enginneer;

public class ToolsProductionAutomation : AutomationObserver
{
    public override bool ShouldExecute() => base.ShouldExecute() && Button.Notification.IsActive();

    public override IEnumerator OnNotificationTriggered()
    {
        yield return Button.Notification.Click();

        var caimToolsButton = new ButtonWrapper(ClaimToolsButton);

        if (caimToolsButton.IsInteractable()) yield return caimToolsButton.Click();

        var closeButton = new ButtonWrapper(CloseButton);
        yield return closeButton.Click();
    }

    private static class Button
    {
        public static readonly ButtonWrapper Notification = new(EngineerGridNotification);
    }
}