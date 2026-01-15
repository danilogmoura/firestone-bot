using System.Collections;
using FireBot.Bot.Component;
using FireBot.Utils;
using Il2CppTMPro;
using UnityEngine;
using static FireBot.Utils.Paths.FirestoneResearch;
using static FireBot.Utils.StringUtils;

namespace FireBot.Bot.Automation.Library
{
    internal static class FirestoneResearchAutomation
    {
        public static IEnumerator Process()
        {
            if (!Buttons.Notification.IsInteractable()) yield break;
            yield return Buttons.Notification.Click();
            LogManager.SubHeader("Firestone Research");

            if (!ResearchPanel.SubmenusWrapper.IsActiveSelf() && ResearchPanel.SelectResearch.IsActiveSelf())
                yield break;

            if (ResearchPanel.Slot0.IsActiveSelf() && Buttons.ButtonClainSlot0.IsActiveSelf())
                yield return Buttons.ButtonClainSlot0.Click();

            if (ResearchPanel.Slot1.IsActiveSelf() && Buttons.ButtonClainSlot1.IsActiveSelf())
                yield return Buttons.ButtonClainSlot1.Click();

            var submenusTransform = ResearchPanel.SubmenusWrapper.Transform;

            for (var i = 0; i < submenusTransform.childCount; i++)
            {
                var tree = submenusTransform.GetChild(i);
                if (!tree.name.StartsWith("tree") || !tree.gameObject.activeSelf) continue;

                for (var j = 0; j < tree.childCount; j++)
                {
                    var slot = new ResearchSlotWrapper(tree.GetChild(j));
                    if (!slot.IsValid() || !ResearchPanel.SelectResearch.IsActiveSelf()) continue;
                    yield return OpenPopup(JoinPath(SubmenusTree, tree.name, slot.Name));

                    if (ResearchPanel.SubmenusWrapper.IsActiveSelf() && Buttons.StartResearch.IsInteractable())
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
                if (_root == null || !_root.gameObject.activeSelf) return false;
                if (!Name.StartsWith("firestoneResearch")) return false;

                var bar = _root.Find("progressBarBg");
                var glow = _root.Find("glow");
                return bar != null && bar.gameObject.activeSelf && (glow == null || !glow.gameObject.activeSelf);
            }

            public string GetTitle()
            {
                var t = _root.Find("researchNameBg/researchName")?.GetComponent<TextMeshProUGUI>();
                return t != null ? t.text : "DESCONHECIDO";
            }
        }

        private static class ResearchPanel
        {
            public static readonly ObjectWrapper SubmenusWrapper = new ObjectWrapper(SubmenusTree);

            public static readonly ObjectWrapper Slot0 = new ObjectWrapper(ResearchPanelDown + "/researchSlot0");

            public static readonly ObjectWrapper Slot1 = new ObjectWrapper(ResearchPanelDown + "/researchSlot1");

            public static readonly ObjectWrapper SelectResearch = new ObjectWrapper(SelectResearchTable);
        }

        private static class Buttons
        {
            public static readonly ButtonWrapper ButtonClainSlot0 =
                new ButtonWrapper(JoinPath(ResearchPanelDown, "researchSlot0/container/claimButton"));

            public static readonly ButtonWrapper ButtonClainSlot1 =
                new ButtonWrapper(JoinPath(ResearchPanelDown, "researchSlot1/container/claimButton"));

            public static ButtonWrapper Notification => new ButtonWrapper(Paths.FirestoneResearch.Notification);
            public static ButtonWrapper Close => new ButtonWrapper(MissionCloseButton);
            public static ButtonWrapper StartResearch => new ButtonWrapper(PopupActivateButton);
        }
    }
}