using System.Collections;
using Firebot.GameModel.Primitives;
using UnityEngine;

namespace Firebot.Core;

public static class Watchdog
{
    private static readonly string[] _nuisancePaths =
    {
        // "Canvas/Popups/DailyReward/ClaimBtn",
        // "Canvas/Popups/SpecialOffer/CloseBtn",
        // "Canvas/Popups/OfflineGains/OkBtn",
        // "Canvas/Errors/ReconnectBtn"
    };

    public static IEnumerator ForceClearAll()
    {
        // Tenta fechar coisas por 3 segundos seguidos
        for (var i = 0; i < 3; i++)
        {
            foreach (var path in _nuisancePaths)
            {
                var btn = new GameButton(path);

                if (!btn.IsVisible()) continue;
                Debug.Log($"[Watchdog] Fechando popup: {path}");
                btn.Click();
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}