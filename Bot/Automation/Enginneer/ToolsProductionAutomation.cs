// Necessário para IEnumerator

using System.Collections;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Bot.Paths.Engineer;

namespace FireBot.Bot.Automation.Enginneer
{
    public static class ToolsProductionAutomation
    {
        public static IEnumerator Process()
        {
            var engineerNotif = new ButtonWrapper(GridNotification);
            if (engineerNotif.Inactive())
            {
                yield break;
            }

            LogManager.SubHeader("Processando Tools Production");
            yield return engineerNotif.Click();

            var caimToolsButton = new ButtonWrapper(ClaimToolsButton);

            if (caimToolsButton.IsInteractable())
            {
                yield return caimToolsButton.Click();
            }

            var closeButton = new ButtonWrapper(CloseButton);
            yield return closeButton.Click();
        }
    }
}