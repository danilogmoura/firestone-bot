using System.Collections;

namespace FireBot.Bot.Automation.Core
{
    public abstract class AutomationObserver
    {
        // We define 50 as the default "middle ground".
        // Virtual allows child classes to override it if they want.
        public virtual int Priority => 50;

        public abstract bool ToogleCondition();

        public abstract IEnumerator OnNotificationTriggered();
    }
}