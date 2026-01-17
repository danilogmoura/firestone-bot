using System.Collections;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using static FireBot.Utils.Paths.BattleRoot;

namespace FireBot.Bot.Automation.Main
{
    internal class OfflineProgressAutomation : IAutomationObserver
    {
        private readonly ButtonWrapper _claimButton = new ButtonWrapper(OfflineProgressPopupClaimButton);
        private ObjectWrapper Popup => new ObjectWrapper(OfflineProgressPopup);

        public bool ToogleCondition()
        {
            return Popup.IsActive();
        }

        public IEnumerator OnNotificationTriggered()
        {
            yield return _claimButton.Click();
        }
    }
}