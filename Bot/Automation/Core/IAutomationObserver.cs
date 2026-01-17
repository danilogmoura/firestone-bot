using System.Collections;

namespace FireBot.Bot.Automation.Core
{
    public interface IAutomationObserver
    {
        bool ToogleCondition();

        IEnumerator OnNotificationTriggered();
    }
}