using System.Collections;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using FireBot.Utils;
using UnityEngine;
using static FireBot.Utils.Paths.FirestoneResearch;
using static FireBot.Utils.StringUtils;

namespace FireBot.Bot.Automation.Library
{
    internal class FirestoneResearchAutomation : AutomationObserver
    {
        public override bool ToogleCondition()
        {
            return Buttons.Notification.IsActive();
        }

        public override IEnumerator OnNotificationTriggered()
        {
            if (!Buttons.Notification.IsInteractable()) yield break;

            yield return Buttons.Notification.Click();

            LogManager.SubHeader("Firestone Research");

            if (!Panel.SubmenusWrapper.IsActive() && Panel.SelectResearch.IsActive())
                yield break;

            if (Panel.Slot0.IsActive() && Buttons.ButtonClainSlot0.IsInteractable())
                yield return Buttons.ButtonClainSlot0.Click();

            if (Panel.Slot1.IsActive() && Buttons.ButtonClainSlot1.IsInteractable())
                yield return Buttons.ButtonClainSlot1.Click();

            var submenusTransform = Panel.SubmenusWrapper.Transform;

            for (var i = 0; i < submenusTransform.childCount; i++)
            {
                var tree = submenusTransform.GetChild(i);
                if (!tree.name.StartsWith("tree") || !tree.gameObject.activeInHierarchy) continue;

                for (var j = 0; j < tree.childCount; j++)
                {
                    var slot = new ResearchSlotWrapper(tree.GetChild(j));
                    if (!slot.IsValid() || !Panel.SelectResearch.IsActive()) continue;
                    yield return OpenPopup(JoinPath(SubmenusTreePath, tree.name, slot.Name));

                    if (Panel.SubmenusWrapper.IsActive() && Buttons.StartResearch.IsInteractable())
                        yield return Buttons.StartResearch.Click();
                }
            }

            yield return Buttons.Close.Click();
        }

        private static IEnumerator OpenPopup(string paths)
        {
            var button = new ButtonWrapper(paths);
            if (!button.IsInteractable()) yield break;
            yield return button.Click();
        }

        private readonly struct ResearchSlotWrapper
        {
            private readonly Transform _root;

            public string Name { get; }

            public ResearchSlotWrapper(Transform t)
            {
                _root = t;
                Name = t.name;
            }

            public bool IsValid()
            {
                if (_root == null || !_root.gameObject.activeInHierarchy) return false;
                if (!Name.StartsWith("firestoneResearch")) return false;

                var bar = _root.Find("progressBarBg");
                var glow = _root.Find("glow");

                return bar != null && bar.gameObject.activeInHierarchy &&
                       (glow == null || !glow.gameObject.activeInHierarchy);
            }
        }

        private static class Panel
        {
            public static readonly ObjectWrapper SubmenusWrapper = new ObjectWrapper(SubmenusTreePath);

            public static readonly ObjectWrapper Slot0 = new ObjectWrapper(ResearchPanelDownPath + "/researchSlot0");

            public static readonly ObjectWrapper Slot1 = new ObjectWrapper(ResearchPanelDownPath + "/researchSlot1");

            public static readonly ObjectWrapper SelectResearch = new ObjectWrapper(SelectResearchTablePath);
        }

        private static class Buttons
        {
            public static readonly ButtonWrapper Notification = new ButtonWrapper(FirestoneResearchNotificationPath);

            public static readonly ButtonWrapper ButtonClainSlot0 =
                new ButtonWrapper(JoinPath(ResearchPanelDownPath, "researchSlot0/container/claimButton"));

            public static readonly ButtonWrapper ButtonClainSlot1 =
                new ButtonWrapper(JoinPath(ResearchPanelDownPath, "researchSlot1/container/claimButton"));

            public static readonly ButtonWrapper Close = new ButtonWrapper(MissionCloseButton);

            public static readonly ButtonWrapper StartResearch = new ButtonWrapper(PopupActivateButton);
        }
    }
}