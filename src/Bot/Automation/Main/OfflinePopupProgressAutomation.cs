using System.Collections;
using Firebot.Bot.Automation.Core;
using Firebot.Bot.Component;
using static Firebot.Utils.Paths.BattleRoot;

namespace Firebot.Bot.Automation.Main;

internal class OfflineProgressAutomation : AutomationObserver
{
    private static readonly ObjectWrapper Popup = new(OfflineProgressPopup);
    private static readonly ButtonWrapper ClaimButton = new(OfflineProgressPopupClaimButton);

    public override int Priority => 20;

    public override bool ShouldExecute() => base.ShouldExecute() && Popup.IsActive();

    public override IEnumerator OnNotificationTriggered()
    {
        yield return ClaimButton.Click();
    }
}