using System.Collections;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using static FireBot.Utils.Paths.BattleRoot;

namespace FireBot.Bot.Automation.Main
{
    internal class OfflineProgressAutomation : IAutomationObserver
    {
        private static readonly ObjectWrapper Popup = new ObjectWrapper(OfflineProgressPopup);
        private static readonly ButtonWrapper ClaimButton = new ButtonWrapper(OfflineProgressPopupClaimButton);

        public bool ToogleCondition()
        {
            return Popup.IsActive();
        }

        public IEnumerator OnNotificationTriggered()
        {
            yield return ClaimButton.Click();
        }
    }
}