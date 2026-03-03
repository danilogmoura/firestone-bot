using System.Collections;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.MainScene;

public static class Upgrades
{
    public static bool IsVisible =>
        new GameElement(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.Root).IsVisible();

    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.CloseBtn).Click();

    private static string LvlUpNumTxt =>
        new GameText(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.GuardianUpgradeLvlTxt).GetParsedText();

    private static IEnumerator ChangeLevelUpModeBtn =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.ChangeLevelUpModeButton).Click();

    /// <summary>
    ///     Sets the upgrade level. The default value is x25.
    /// </summary>
    public static IEnumerator SetUpgradeLevel()
    {
        if (!IsVisible) yield break;

        var currentLevel = LvlUpNumTxt;
        if (currentLevel == "x25") yield break;


        while (currentLevel != "x25")
        {
            yield return ChangeLevelUpModeBtn;
            currentLevel = LvlUpNumTxt;
        }
    }
}