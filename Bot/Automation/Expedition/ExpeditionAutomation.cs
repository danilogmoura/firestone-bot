using System.Collections;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Bot.Paths.Expedition;

namespace FireBot.Bot.Automation.Expedition
{
    public abstract class ExpeditionAutomation
    {
        public static IEnumerator Process()
        {
            if (Buttons.Notification.Inactive())
            {
                yield break;
            }

            LogManager.SubHeader("Processando Expedition");
            yield return Buttons.Notification.Click();

            if (Expeditions.CurrentExpedition.IsActive() && Expeditions.CurrentExpedition.IsCompleted())
            {
                yield return Expeditions.CurrentExpedition.CollectRewards();
            }

            if (Expeditions.CurrentExpedition.Inactive() && Expeditions.PendingExpedition.IsActive())
            {
                yield return Expeditions.PendingExpedition.StartExpedition();
            }

            {
                yield return Buttons.Close.Click();
            }
        }


        private static class Buttons
        {
            public static ButtonWrapper Notification => new ButtonWrapper(ExpeditionNotification);
            public static ButtonWrapper Close => new ButtonWrapper(ExpeditionCloseButton);
        }

        private static class Expeditions
        {
            public static CurrentExpedition CurrentExpedition => new CurrentExpedition();
            public static PendingExpedition PendingExpedition => new PendingExpedition();
        }

        private class CurrentExpedition : ObjectWrapper
        {
            public CurrentExpedition() : base(CurrenteExpedition)
            {
            }

            private static TextMeshProWrapperUGUI TimeLabel =>
                new TextMeshProWrapperUGUI(CurrenteExpedition, "expeditionProgressBg/timeLeftText");

            private static ButtonWrapper ClaimButton => new ButtonWrapper(CurrenteExpedition, "claimButton");

            public bool IsCompleted()
            {
                var text = TimeLabel.GetParsedText();
                return text.Contains("Completed");
            }

            public IEnumerator CollectRewards()
            {
                if (ClaimButton.IsInteractable())
                {
                    yield return ClaimButton.Click();
                }
            }
        }

        private class PendingExpedition : ObjectWrapper
        {
            public PendingExpedition() : base(Paths.Expedition.PendingExpedition)
            {
            }

            private static ButtonWrapper StartButton => new ButtonWrapper(Paths.Expedition.PendingExpedition, "startButton");

            public IEnumerator StartExpedition()
            {
                if (StartButton.IsInteractable())
                {
                    yield return StartButton.Click();
                }
            }
        }
    }
}