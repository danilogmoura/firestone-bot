using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.Bot.Automation.Core;
using Firebot.Bot.Component;
using Firebot.Utils;
using static Firebot.Utils.Paths;
using static Firebot.Utils.StringUtils;

namespace Firebot.Bot.Automation.Main;

public class CloseEventsAutomation : AutomationObserver
{
    private const string CloseButtonPath = "bg/closeButton";

    private readonly List<string> _eventsPath = new();

    public override string SectionTitle => "Close Events";

    public override int Priority => 1;

    public override bool ShouldExecute()
    {
        return base.ShouldExecute() && HasActiveEvent();
    }

    public override IEnumerator OnNotificationTriggered()
    {
        yield return CloseEvents();
    }

    private IEnumerator CloseEvents()
    {
        foreach (var rootPath in _eventsPath)
        {
            var closeButton = new ButtonWrapper(JoinPath(rootPath, CloseButtonPath));

            if (closeButton.IsActive())
                yield return closeButton.Click();
        }
    }

    private bool HasActiveEvent()
    {
        _eventsPath.Clear();
        var events = new ObjectWrapper(EventsPopupPath);

        if (!events.IsActive() || !events.HasChildren()) return false;

        for (var i = 0; i < events.ChildCount(); i++)
        {
            var eventFolder = events.GetChild(i);
            if (!eventFolder.gameObject.activeInHierarchy) continue;
            _eventsPath.Add(JoinPath(EventsPopupPath, eventFolder.name));
        }

        return _eventsPath.Any();
    }
}