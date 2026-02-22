using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.Oracle;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;

namespace Firebot.Behaviors.Town;

public class OracleTask : BotTask
{
    public override string NotificationPath => Paths.BattleLoc.NotificationsLoc.OracleRitualsBtn;

    public override IEnumerator Execute()
    {
        yield return Notifications.OracleRituals;
        var rituals = new Rituals();
        yield return rituals.Claim();
        yield return rituals.Start();
        NextRunTime = rituals.NextRunTime();
        yield return Oracle.Close;
    }
}