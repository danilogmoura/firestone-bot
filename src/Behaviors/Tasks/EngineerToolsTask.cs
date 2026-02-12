using System.Collections;
using Firebot.GameModel.Features.Engineer.Tools;
using Firebot.GameModel.Shared;
using Logger = Firebot.Core.Logger;

namespace Firebot.Behaviors.Tasks;

public class EngineerToolsTask : BotTask
{
    public override IEnumerator Execute()
    {
        yield return new MainHUD().TownButton.Click();

        var townHUD = new TownHUD();
        yield return townHUD.EngineerButton.Click();
        yield return new GaragePopup().EngineerButton.Click();

        var engineerSubmenu = new EngineerSubmenu();
        var claimToolsButton = engineerSubmenu.ClaimToolsButton;
        yield return claimToolsButton.Click();

        NextRunTime = engineerSubmenu.FindNextRunTime;
        Logger.Debug($"Engineer tools are on cooldown, next run time: {NextRunTime}");

        yield return engineerSubmenu.CloseButton.Click();
        yield return townHUD.CloseButton.Click();
    }
}