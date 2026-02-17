using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.Engineer.Tools;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;

namespace Firebot.Behaviors;

public class EngineerToolsTask : BotTask
{
    public override string NotificationPath => Paths.BattleLoc.NotificationsLoc.Engineer;

    public override IEnumerator Execute()
    {
        yield return Notifications.Engineer;
        yield return EngineerSubmenu.ClaimTools;
        NextRunTime = EngineerSubmenu.NextRunTime;
        yield return EngineerSubmenu.Close;
    }
}