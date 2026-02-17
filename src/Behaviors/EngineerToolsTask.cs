using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Engineer.Tools;
using Firebot.GameModel.Shared;

namespace Firebot.Behaviors;

public class EngineerToolsTask : BotTask
{
    public override IEnumerator Execute()
    {
        yield return Notifications.Engineer;

        yield return EngineerSubmenu.ClaimTools;
        NextRunTime = EngineerSubmenu.FindNextRunTime;

        yield return EngineerSubmenu.Close;
        yield return GaragePopup.Close;
    }
}