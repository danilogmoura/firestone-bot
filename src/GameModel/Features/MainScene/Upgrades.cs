using System;
using System.Collections;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.MainScene;

public static class Upgrades
{
    private static string _cachedLevel = string.Empty;

    public static bool IsVisible =>
        new GameElement(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.Root).IsVisible();

    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.CloseBtn).Click();

    private static string LvlUpNumTxt =>
        new GameText(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.ChangeLevelUpModeBtnTxt).GetParsedText();

    private static IEnumerator ChangeLevelUpModeBtn =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MainSceneLoc.UpgradesLoc.ChangeLevelUpModeButton).Click();

    /// <summary>
    ///     Sets the upgrade level. The default value is x25.
    /// </summary>
    public static IEnumerator SetUpgradeLevel()
    {
        if (!IsVisible) yield break;
        var currentLevel = LvlUpNumTxt;

        if (!string.IsNullOrEmpty(currentLevel) &&
            string.Equals(currentLevel, _cachedLevel, StringComparison.Ordinal))
            yield break;

        if (!string.IsNullOrEmpty(currentLevel) && currentLevel.Contains("x100"))
        {
            yield return ChangeLevelUpModeBtn;
            currentLevel = LvlUpNumTxt;
            _cachedLevel = currentLevel;
            yield break;
        }

        var attempts = 0;
        const int maxAttempts = 10; // Prevent infinite loop in case of unexpected issues

        while (attempts < maxAttempts)
        {
            yield return ChangeLevelUpModeBtn;
            currentLevel = LvlUpNumTxt;

            if (string.IsNullOrEmpty(currentLevel))
                break;

            if (string.Equals(currentLevel, _cachedLevel, StringComparison.Ordinal))
                break;

            _cachedLevel = currentLevel;

            if (currentLevel.Contains("x100"))
            {
                yield return ChangeLevelUpModeBtn;
                currentLevel = LvlUpNumTxt;
                if (!string.IsNullOrEmpty(currentLevel))
                    _cachedLevel = currentLevel;
                break;
            }

            attempts++;
        }
    }
}