using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Guild.Expeditions;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;

namespace Firebot.Behaviors.Guild;

public class ExpeditionTask : BotTask
{
    public override string NotificationPath => Paths.BattleLoc.NotificationsLoc.ExpeditionsBtn;

    public override IEnumerator Execute()
    {
        yield return Notifications.Expeditions;
        yield return Expedition.Claim;
        yield return Expedition.Start;

        NextRunTime = Expedition.IsExpeditionActive ? Expedition.CurrentRunTime : Expedition.NextRunTime;

        yield return Expedition.Close;
    }
}