using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.Bot.Automation.Core;
using Firebot.Bot.Component;
using Firebot.Bot.Component.TextMeshPro;
using static Firebot.Utils.Paths;
using static Firebot.Utils.StringUtils;

namespace Firebot.Bot.Automation.Main;

// menusRoot/menuCanvasParent/SafeArea/menuCanvas/events/AnniversaryEventPromotional
// menusRoot/menuCanvasParent/SafeArea/menuCanvas/events/AnniversaryEventPromotional/bg/closeButton
// menusRoot/menuCanvasParent/SafeArea/menuCanvas/events/AnniversaryEventPromotional/bg/titleBg/menuTitle
public class CloseEventsAutomation : AutomationObserver
{
    private readonly List<string> _eventsPath = new();

    public override string SectionTitle => "Close Events";

    public override int Priority => 25;

    public override bool ShouldExecute() => base.ShouldExecute() && HasActiveEvent();

    public override IEnumerator OnNotificationTriggered()
    {
        yield return CloseEvents();
    }

    private IEnumerator CloseEvents()
    {
        foreach (var rootPath in _eventsPath) { }

        yield break;
    }

    private bool HasActiveEvent()
    {
        _eventsPath.Clear();
        var events = new ObjectWrapper(EventsPopupPath);

        if (!events.IsActive() || !events.HasChildren()) return false;

        for (var i = 0; i < events.ChildCount(); i++)
        {
            var eventFolder = events.GetChild(i);

            var eventFolderName = eventFolder.name;

            Log($"=>>>>>>>>>>>>>>>>>> eventFolderName: {eventFolderName}");

            var textUI = new TextUI(JoinPath(EventsPopupPath, eventFolderName, "bg/titleBg/menuTitle"));
            Log($"=>>>>>>>>>>>>>>>>>> textUI.Text: {textUI.Text}");

            if (!eventFolder.gameObject.activeInHierarchy) continue;
            _eventsPath.Add(JoinPath(EventsPopupPath, eventFolder.name));
        }

        return _eventsPath.Any();
    }
}