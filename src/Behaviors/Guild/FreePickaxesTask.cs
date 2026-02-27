using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Guild.Shop;
using Firebot.GameModel.Shared;
using MelonLoader;

namespace Firebot.Behaviors.Guild;

public class FreePickaxesTask : BotTask
{
    private MelonPreferences_Entry<int> _pickaxeClaimThreshold;

    public int PickaxeClaimThreshold => _pickaxeClaimThreshold?.Value ?? 1;

    protected override void OnConfigure(MelonPreferences_Category category)
    {
        if (_pickaxeClaimThreshold != null) return;

        _pickaxeClaimThreshold = category.CreateEntry(
            "pickaxe_claim_threshold",
            30,
            "Pickaxe Claim Threshold",
            "Minimum number of free pickaxes required before claiming. " +
            "Set to 1 to claim as soon as available, or up to 30 to wait for maximum. " +
            "Default is 30 (wait for maximum)."
        );
    }

    public override IEnumerator Execute()
    {
        yield return Notifications.FreePickaxes;

        if (FreePickaxes.Quantity >= PickaxeClaimThreshold)
            yield return FreePickaxes.Claim;

        NextRunTime = FreePickaxes.NextRunTime;
        yield return GuildShop.Close;
    }
}