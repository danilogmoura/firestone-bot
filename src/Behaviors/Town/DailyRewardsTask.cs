using System.Collections;
using Firebot.Core;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.Oracle;
using Firebot.GameModel.Shared;
using UnityEngine;

namespace Firebot.Behaviors.Town;

public class DailyRewardsTask : BotTask
{
    private const float MenuOpenDelay = 2f;

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

        // Fail-closed here, unlike BotTask.IsLevelLocked: the Oracle gift is a bonus on top of the check-in,
        // and the menu only exists at LevelRequirements.Oracle — with no level read there is no reason to
        // trust that it is there.
        if (!PlayerStats.TryGetCharacterLevel(out var level) || level < LevelRequirements.Oracle)
            yield break;

        yield return Notifications.OraclesGift;
        yield return new WaitForSeconds(MenuOpenDelay);
        yield return OracleStore.ClaimGift;
        NextRunTime = OracleStore.NextRunTime;
        yield return OracleStore.Close;
    }
}
