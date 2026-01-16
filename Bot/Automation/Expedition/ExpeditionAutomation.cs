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

            if (Expeditions.CurrentExpedition.IsCompleted())
                yield return Expeditions.CurrentExpedition.CollectRewards();

            if (!Expeditions.CurrentExpedition.IsActive() && Expeditions.PendingExpedition.IsActive())
                yield return Expeditions.PendingExpedition.StartExpedition();

            yield return Buttons.Close.Click();
        }

        private static class Buttons
        {
            public static ButtonWrapper Notification => new ButtonWrapper(ExpeditionNotification);
            public static ButtonWrapper Close => new ButtonWrapper(CloseButton);
        }

        private static class Expeditions
        {
            public static CurrentExpeditionSection CurrentExpedition => new CurrentExpeditionSection();
            public static PendingExpeditionSection PendingExpedition => new PendingExpeditionSection();
        }

        private class CurrentExpeditionSection : ObjectWrapper
        {
            public CurrentExpeditionSection() : base(CurrentExpedition)
            {
            }

            private ButtonWrapper ClaimButton => new ButtonWrapper(JoinPath(CurrentExpedition, "claimButton"));

            public bool IsCompleted()
            {
                var timeLabel =
                    new TextMeshProUGUIWrapper(JoinPath(CurrentExpedition, "expeditionProgressBg/timeLeftText"));

                if (!IsActive() && !timeLabel.Exists()) return false;

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