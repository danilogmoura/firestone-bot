using System.Collections;
using Firebot.Automation.Core;
using Firebot.GameModel.Wrappers;
using static Firebot.Core.Paths.BattleRoot;

namespace Firebot.Automation.Main;

internal class OfflinePopupProgressAutomation : AutomationObserver
{
    private static readonly TransformWrapper Popup = new(OfflineProgressPopup);
    private static readonly ButtonWrapper ClaimButton = new(OfflineProgressPopupClaimButton);

    private bool _hasExecutedSuccessfully;

    public override int Priority => 20;

    public override bool ShouldExecute() => !_hasExecutedSuccessfully && base.ShouldExecute() && Popup.IsActive();

    public override IEnumerator OnNotificationTriggered()
    {
        yield return ClaimButton.Click();
        _hasExecutedSuccessfully = true;
    }
}