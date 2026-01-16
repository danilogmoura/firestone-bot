using System.Collections;
using FireBot.Bot.Component;
using static FireBot.Utils.Paths.BattleRoot;

namespace FireBot.Bot.Automation.Main
{
    public abstract class OfflineProgressAutomation
    {
        public static IEnumerator Process()
        {
            var popup = new ObjectWrapper(OfflineProgressPopup);
            if (!popup.IsActiveSelf()) yield break;

            yield return new ButtonWrapper(OfflineProgressPopupClaimButton)?.Click();
        }
    }
}