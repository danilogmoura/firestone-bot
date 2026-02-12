using System.Collections;
using Firebot.GameModel.Features.MapMissions;
using Firebot.GameModel.Features.MapMissions.WarfrontCampaign;
using Firebot.GameModel.Shared;
using UnityEngine;
using static Firebot.Core.BotSettings;

namespace Firebot.Behaviors.Tasks;

public class WarfrontCampaignLoot : BotTask
{
    public override IEnumerator Execute()
    {
        yield return new MainHUD().MapButton.Click();
        yield return new MapMissionHUD().WarfrontCampaignButton.Click();

        var loot = new Loot();
        var claimToolsButton = loot.ClaimToolsButton;
        yield return claimToolsButton.Click();

        NextRunTime = loot.FindNextRunTime;
        yield return new WaitForSeconds(InteractionDelay);
        yield return new MapMissionHUD().CloseButton.Click();
    }
}