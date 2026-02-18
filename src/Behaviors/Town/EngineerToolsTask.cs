using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.Engineer;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;

namespace Firebot.Behaviors.Town;

public class EngineerToolsTask : BotTask
{
    public override string NotificationPath => Paths.BattleLoc.NotificationsLoc.EngineerBtn;

    public override IEnumerator Execute()
    {
        yield return Notifications.Engineer;
        yield return Engineer.Claim;
        NextRunTime = Engineer.NextRunTime;
        yield return Engineer.Close;
    }
}