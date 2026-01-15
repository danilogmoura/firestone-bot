using System.Collections;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Utils.Paths.Expedition;
using static FireBot.Utils.StringUtils;

namespace FireBot.Bot.Automation.Expedition
{
    public abstract class ExpeditionAutomation
    {
        public static IEnumerator Process()
        {
            if (!Buttons.Notification.IsActive()) yield break;

            LogManager.SubHeader("Expedition");
            yield return Buttons.Notification.Click();

            if (Expeditions.CurrentExpeditionSection.IsCompleted())
                LogManager.Info("Expedition completed");
            // yield return Expeditions.CurrentExpeditionSection.CollectRewards();

            if (!Expeditions.CurrentExpeditionSection.IsActive() && Expeditions.PendingExpeditionSection.IsActive())
            {
                LogManager.Info("$No active expedition. Starting a new one.");
                yield return Expeditions.PendingExpeditionSection.StartExpedition();
            }

            yield return Buttons.Close.Click();
        }

        private static class Buttons
        {
            public static ButtonWrapper Notification => new ButtonWrapper(ExpeditionNotification);
            public static ButtonWrapper Close => new ButtonWrapper(ExpeditionCloseButton);
        }

        private static class Expeditions
        {
            public static CurrentExpeditionSection CurrentExpeditionSection => new CurrentExpeditionSection();
            public static PendingExpeditionSection PendingExpeditionSection => new PendingExpeditionSection();
        }

        private class CurrentExpeditionSection : ObjectWrapper
        {
            public CurrentExpeditionSection() : base(CurrenteExpedition)
            {
            }

            private ButtonWrapper ClaimButton => new ButtonWrapper(JoinPath(CurrenteExpedition, "claimButton"));

            public bool IsCompleted()
            {
                var timeLabel =
                    new TextMeshProUGUIWrapper(JoinPath(CurrenteExpedition, "expeditionProgressBg/timeLeftText"));

                if (!IsActiveSelf() && !timeLabel.Exists()) return false;

                var text = timeLabel.GetParsedText();
                return text.Contains("Completed");
            }

            public IEnumerator CollectRewards()
            {
                if (ClaimButton.IsInteractable()) yield return ClaimButton.Click();
            }
        }

        private class PendingExpeditionSection : ObjectWrapper
        {
            public PendingExpeditionSection() : base(PendingExpedition)
            {
            }

            private ButtonWrapper StartButton => new ButtonWrapper(JoinPath(PendingExpedition, "startButton"));

            public IEnumerator StartExpedition()
            {
                if (StartButton.IsInteractable())
                    yield return StartButton.Click();
            }
        }
    }
}