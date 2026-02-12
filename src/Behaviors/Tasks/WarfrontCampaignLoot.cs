using System.Collections;
using Firebot.GameModel.Features.MapMissions;
using Firebot.GameModel.Features.MapMissions.WarfrontCampaign;
using Firebot.GameModel.Shared;
using UnityEngine;
using static Firebot.Core.BotSettings;
using Logger = Firebot.Core.Logger;

namespace Firebot.Behaviors.Tasks;

public class WarfrontCampaignLoot : BotTask
{
    public override IEnumerator Execute()
    {
        new MainHUD().MapButton.Click();
        yield return new WaitForSeconds(InteractionDelay);

        new MapMissionHUD().WarfrontCampaignButton.Click();
        yield return new WaitForSeconds(InteractionDelay);

        var loot = new Loot();

        if (!loot.IsVisible())
        {
            Logger.Debug("Warfront Campaign loot not available.");
            yield break;
        }

        var claimToolsButton = loot.ClaimToolsButton;
        if (claimToolsButton.IsClickable())
        {
            claimToolsButton.Click();

            Logger.Debug("Claimed Warfront Campaign tools.");
            yield return new WaitForSeconds(InteractionDelay);
        }

        NextRunTime = loot.FindNextRunTime;
        yield return new WaitForSeconds(InteractionDelay);

        new MapMissionHUD().CloseButton.Click();
        yield return new WaitForSeconds(InteractionDelay);
    }
}