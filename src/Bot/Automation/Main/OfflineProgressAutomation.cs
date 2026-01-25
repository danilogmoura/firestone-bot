using System.Collections;
using Firebot.Bot.Automation.Core;
using Firebot.Bot.Component;
using Firebot.Utils;
using static Firebot.Utils.Paths.BattleRoot;

namespace Firebot.Bot.Automation.Main
{
    internal class OfflineProgressAutomation : AutomationObserver
    {
        private static readonly ObjectWrapper Popup = new ObjectWrapper(OfflineProgressPopup);
        private static readonly ButtonWrapper ClaimButton = new ButtonWrapper(OfflineProgressPopupClaimButton);

        public override string SectionName => "Offline Progress";

        public override bool ToogleCondition()
        {
            return Popup.IsActive();
        }

        public override IEnumerator OnNotificationTriggered()
        {
            LogManager.SubHeader("Offline Progress");
            yield return ClaimButton.Click();
        }
    }
}