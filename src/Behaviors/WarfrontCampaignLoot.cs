using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.MapMissions.WarfrontCampaign;
using Firebot.GameModel.Shared;
using UnityEngine;
using static Firebot.Core.BotSettings;

namespace Firebot.Behaviors;

public class WarfrontCampaignLoot : BotTask
{
    public override IEnumerator Execute()
    {
        yield return Notifications.WarfrontCampaign;
        yield return WarfrontLoot.ClaimTools;

        NextRunTime = WarfrontLoot.FindNextRunTime;
        yield return new WaitForSeconds(InteractionDelay);
    }
}