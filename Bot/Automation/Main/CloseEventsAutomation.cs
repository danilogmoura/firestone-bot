using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using FireBot.Utils;
using MelonLoader;
using static FireBot.Utils.Paths;
using static FireBot.Utils.StringUtils;

namespace FireBot.Bot.Automation.Main
{
    public class CloseEventsAutomation : AutomationObserver
    {
        private const string CloseButtonPath = "bg/closeButton";

        private readonly List<string> _eventsPath = new List<string>();

        public override int Priority => 26;

        public override bool ToogleCondition()
        {
            return HasActiveEvent();
        }

        public override IEnumerator OnNotificationTriggered()
        {
            LogManager.SubHeader("Close Events");
            yield return CloseEvents();
        }

        private IEnumerator CloseEvents()
        {
            foreach (var rootPath in _eventsPath)
            {
                var closeButton = new ButtonWrapper(JoinPath(rootPath, CloseButtonPath));

                MelonLogger.Msg(JoinPath(rootPath, CloseButtonPath));

                if (closeButton.IsActive())
                    yield return closeButton.Click();
            }
        }

        private bool HasActiveEvent()
        {
            _eventsPath.Clear();
            var events = new ObjectWrapper(EventsPopupPath);

            if (!events.IsActive() || !events.HasChilden()) return false;

            for (var i = 0; i < events.ChildCount(); i++)
            {
                var eventFolder = events.GetChild(i);
                if (!eventFolder.gameObject.activeInHierarchy) continue;
                _eventsPath.Add(JoinPath(EventsPopupPath, eventFolder.name));
            }

            return _eventsPath.Any();
        }
    }
}