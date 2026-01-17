using System.Collections;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using FireBot.Utils;
using Il2CppTMPro;
using UnityEngine;
using static FireBot.Utils.Paths.FirestoneResearch;
using static FireBot.Utils.StringUtils;

namespace FireBot.Bot.Automation.Library
{
    internal class FirestoneResearchAutomation : IAutomationObserver
    {
        public bool ToogleCondition()
        {
            return Buttons.Notification.IsActive();
        }

        public IEnumerator OnNotificationTriggered()
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
                    yield return OpenPopup(JoinPath(SubmenusTree, tree.name, slot.Name));

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

            public ResearchSlotWrapper(Transform t)
            {
                _root = t;
            }

            public string Name => _root.name;

            public bool IsValid()
            {
                if (_root == null || !_root.gameObject.activeInHierarchy) return false;
                if (!Name.StartsWith("firestoneResearch")) return false;

                var bar = _root.Find("progressBarBg");
                var glow = _root.Find("glow");
                return bar != null && bar.gameObject.activeInHierarchy &&
                       (glow == null || !glow.gameObject.activeInHierarchy);
            }

            public string GetTitle()
            {
                var t = _root.Find("researchNameBg/researchName")?.GetComponent<TextMeshProUGUI>();
                return t != null ? t.text : "DESCONHECIDO";
            }
        }

        private static class Panel
        {
            public static ObjectWrapper SubmenusWrapper => new ObjectWrapper(SubmenusTree);

            public static ObjectWrapper Slot0 => new ObjectWrapper(ResearchPanelDown + "/researchSlot0");

            public static ObjectWrapper Slot1 => new ObjectWrapper(ResearchPanelDown + "/researchSlot1");

            public static ObjectWrapper SelectResearch => new ObjectWrapper(SelectResearchTable);
        }

        private static class Buttons
        {
            public static readonly ButtonWrapper Notification = new ButtonWrapper(FirestoneResearchNotification);

            public static ButtonWrapper ButtonClainSlot0 =>
                new ButtonWrapper(JoinPath(ResearchPanelDown, "researchSlot0/container/claimButton"));

            public static ButtonWrapper ButtonClainSlot1 =>
                new ButtonWrapper(JoinPath(ResearchPanelDown, "researchSlot1/container/claimButton"));

            public static ButtonWrapper Close => new ButtonWrapper(MissionCloseButton);
            public static ButtonWrapper StartResearch => new ButtonWrapper(PopupActivateButton);
        }
    }
}