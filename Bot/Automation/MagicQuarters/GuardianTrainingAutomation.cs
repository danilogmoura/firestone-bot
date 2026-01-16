using System.Collections;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Utils.Paths.GuardianTraining;
using static FireBot.Utils.StringUtils;

namespace FireBot.Bot.Automation.MagicQuarters
{
    public static class GuardianTrainingAutomation
    {
        public static IEnumerator Process()
        {
            if (!Button.GuardianTrainingNotification.IsInteractable())
                yield break;

            yield return Button.GuardianTrainingNotification?.Click();

            if (!Object.Panel.IsActiveSelf()) yield break;

            LogManager.SubHeader("Guardian Training");

            if (Object.CooldownOn == null || Object.CooldownOn.IsActiveSelf())
            {
                yield return Button.Close?.Click();
                yield break;
            }

            var guardianList = new ObjectWrapper(GuardianList).Transform;
            for (var i = 0; i < guardianList.childCount; i++)
            {
                var guardianRoot = guardianList.GetChild(i);
                var starsParent = guardianRoot.Find("starsParent");

                if (Object.CooldownOn.IsActiveSelf()) break;

                if (Button.Training != null &&
                    (!starsParent.gameObject.activeSelf || !Button.Training.IsInteractable())) continue;

                yield return Button.Training?.Click();

                yield return new ButtonWrapper(JoinPath(GuardianList, guardianRoot.name))?.Click();
            }

            yield return Button.Close?.Click();
        }

        private readonly struct Button
        {
            public static ButtonWrapper GuardianTrainingNotification => new ButtonWrapper(Notification);

            public static ButtonWrapper Close => new ButtonWrapper(CloseButton);

            public static ButtonWrapper Training => new ButtonWrapper(TrainingButton);
        }

        private readonly struct Object
        {
            public static ObjectWrapper CooldownOn => new ObjectWrapper(JoinPath(TrainingButton, "cooldownOn"));

            public static ObjectWrapper CooldownOff => new ObjectWrapper(JoinPath(TrainingButton, "cooldownOff"));

            public static ObjectWrapper Panel => new ObjectWrapper(MenuMagicQuarters);
        }
    }
}