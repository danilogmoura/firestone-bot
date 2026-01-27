using System.Collections;
using System.Collections.Generic;
using Firebot.Bot.Automation.Core;
using Firebot.Bot.Component;
using Firebot.Bot.Component.TextMeshPro;
using UnityEngine;
using static Firebot.Core.BotSettings;
using static Firebot.Utils.Paths;
using static Firebot.Utils.StringUtils;

namespace Firebot.Bot.Automation.Main;

public class CloseEventPromotionalAutomation : AutomationObserver
{
    // Path to the events popup root and its closed state
    private readonly Dictionary<string, bool> _eventsPopup = new()
    {
        { "AnniversaryEventPromotional", true }
    };

    public override int Priority => 25;

    public override bool ShouldExecute() => base.ShouldExecute() && _eventsPopup.ContainsValue(true);

    public override IEnumerator OnNotificationTriggered()
    {
        var events = new ObjectWrapper(EventsPopupPath);

        if (!events.IsActive() || !events.HasChildren()) yield return new WaitForSeconds(InteractionDelay);

        for (var i = 0; i < events.ChildCount(); i++)
        {
            var eventFolder = events.GetChild(i);
            if (!eventFolder.gameObject.activeInHierarchy) continue;

            var eventFolderName = eventFolder.name;
            var popupWrapper = new PopupWrapper(JoinPath(EventsPopupPath, eventFolderName));

            if (!popupWrapper.IsActive()) continue;

            var titleTextText = popupWrapper.TitleText.Text;
            Log(
                $"[{i + 1}/{events.ChildCount()}] Event found: name=\"{eventFolderName}\", title=\"{titleTextText}\"");

            if (!_eventsPopup.ContainsKey(eventFolderName) || !_eventsPopup[eventFolderName]) continue;

            yield return popupWrapper.CloseButton.Click();
            _eventsPopup[eventFolderName] = popupWrapper.IsActive();

            Log($"Closed event popup: name=\"{eventFolderName}\", title=\"{titleTextText}\"");
        }
    }

    private class PopupWrapper : ComponentWrapper<Transform>
    {
        public PopupWrapper(string path) : base(path) { }

        public ButtonWrapper CloseButton => new(JoinPath(Path, "bg/closeButton"));

        public TextUI TitleText => new(JoinPath(Path, "bg/titleBg/menuTitle"));
    }
}