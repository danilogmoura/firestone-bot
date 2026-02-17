using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Engineer.Tools;
using Firebot.GameModel.Shared;

namespace Firebot.Behaviors;

public class EngineerToolsTask : BotTask
{
    public override IEnumerator Execute()
    {
        yield return MainHUD.TownButton.Click();
        yield return TownHUD.EngineerButton.Click();
        yield return GaragePopup.EngineerButton.Click();

        yield return EngineerSubmenu.ClaimToolsButton.Click();
        NextRunTime = EngineerSubmenu.FindNextRunTime;

        yield return EngineerSubmenu.CloseButton.Click();
        yield return GaragePopup.CloseButton.Click();
        yield return TownHUD.CloseButton.Click();
    }
}