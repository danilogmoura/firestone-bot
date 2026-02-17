using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Map;
using Firebot.GameModel.Features.Map.WarfrontCampaign;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;

namespace Firebot.Behaviors;

public class WarfrontCampaignLoot : BotTask
{
    public override string NotificationPath => Paths.BattleLoc.NotificationsLoc.WarfrontCampaign;

    public override IEnumerator Execute()
    {
        yield return Notifications.WarfrontCampaign;
        yield return WarfrontLoot.ClaimTools;
        NextRunTime = WarfrontLoot.NextRunTime;
        yield return MapMission.Close;
    }
}