using System.Collections;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Bot.Paths.WarfrontCampaign;

namespace FireBot.Bot.Automation.Enginneer
{
    public static class WarfrontCampaignAtomation
    {
        public static IEnumerator Process()
        {
            var componentNotification = new ButtonWrapper(GridWarfrontCampaign);
            if (componentNotification.Inactive())
            {
                yield break;
            }

            LogManager.SubHeader("Processando Warfront Campaign");
            yield return componentNotification.Click();

            var claimToolsButton = new ButtonWrapper(ClaimToolsButton);
            var closeButton = new ButtonWrapper(CloseButton);

            if (claimToolsButton.IsInteractable())
            {
                yield return claimToolsButton.Click();
            }

            yield return closeButton.Click();
        }
    }
}