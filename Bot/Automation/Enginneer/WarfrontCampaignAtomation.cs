using System.Collections;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Utils.Paths.WarfrontCampaign;

namespace FireBot.Bot.Automation.Enginneer
{
    public static class WarfrontCampaignAtomation
    {
        public static IEnumerator Process()
        {
            var componentNotification = new ButtonWrapper(GridNotification);
            if (!componentNotification.IsActive()) yield break;

            LogManager.SubHeader("Warfront Campaign Scrolls");
            yield return componentNotification.Click();

            var claimToolsButton = new ButtonWrapper(ClaimToolsButton);
            var closeButton = new ButtonWrapper(CloseButton);

            if (claimToolsButton.IsInteractable()) yield return claimToolsButton.Click();

            yield return closeButton.Click();
        }
    }
}