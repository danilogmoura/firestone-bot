using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.Oracle;
using Firebot.GameModel.Shared;
using UnityEngine;

namespace Firebot.Behaviors.Town;

public class DailyRewardsTask : BotTask
{
    private const float MenuOpenDelay = 2f;
    private const int MinimumLevelForOracleGift = 200;

    public override IEnumerator Execute()
    {
        yield return Notifications.MysteryBox;
        yield return new WaitForSeconds(MenuOpenDelay);
        yield return Store.ClaimMysteryBox;
        NextRunTime = Store.MysteryBoxNextRunTime;
        yield return Store.Close;

        yield return Notifications.CheckIn;
        yield return new WaitForSeconds(MenuOpenDelay);
        yield return Store.ClaimCheckIn;
        NextRunTime = Store.CheckInNextRunTime;
        yield return Store.Close;

        if (PlayerAvatar.CharacterLevel >= MinimumLevelForOracleGift)
        {
            yield return Notifications.OraclesGift;
            yield return new WaitForSeconds(MenuOpenDelay);
            yield return OracleStore.ClaimGift;
            NextRunTime = OracleStore.NextRunTime;
            yield return OracleStore.Close;
        }
    }
}