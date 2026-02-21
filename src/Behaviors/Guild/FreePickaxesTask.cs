using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Guild.Shop;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;

namespace Firebot.Behaviors.Guild;

public class FreePickaxesTask : BotTask
{
    public override string NotificationPath => Paths.BattleLoc.NotificationsLoc.FreePickaxesBtn;

    public override IEnumerator Execute()
    {
        yield return Notifications.FreePickaxes;
        yield return FreePickaxes.Claim;
        NextRunTime = FreePickaxes.NextRunTime;
        yield return GuildShop.Close;
    }
}