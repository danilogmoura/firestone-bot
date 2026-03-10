using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Map;
using Firebot.GameModel.Features.Map.LiberationMissions;
using Firebot.GameModel.Shared;

namespace Firebot.Behaviors.Map;

public class LiberationMissionsTask : BotTask
{
    public override IEnumerator Execute()
    {
        yield return Notifications.WarfrontCampaign;
        yield return DailyMissionsPopup.DailyMissions;
        yield return DailyMissionsPopup.LiberationMissions;

        yield return LiberationMissionsPopup.StartMission();
        yield return LiberationMissionsPopup.Close;

        NextRunTime = DailyMissionsPopup.NextRunTime;
        yield return DailyMissionsPopup.Close;
        yield return MapMission.Close;
    }
}