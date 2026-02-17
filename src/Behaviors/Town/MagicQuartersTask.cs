using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.Engineer.Tools;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using MelonLoader;

namespace Firebot.Behaviors.Town;

public class MagicQuartersTask : BotTask
{
    private MelonPreferences_Entry<int> _guardianIndex;
    private MelonPreferences_Entry<bool> _useStrangeDust;

    public override string NotificationPath => Paths.BattleLoc.NotificationsLoc.GuardianTrainingBtn;

    protected override void OnConfigure(MelonPreferences_Category category)
    {
        if (_guardianIndex != null) return;

        _guardianIndex = category.CreateEntry(
            "guardian_index",
            1,
            "Guardian Index",
            "Select guardian index for training. Use 0-3. Default is 1.\n" +
            "0=Grace, 1=Vermilion, 2=Ankaa, 3=Azhar."
        );

        _useStrangeDust = category.CreateEntry(
            "use_strange_dust",
            false,
            "Use Strange Dust",
            "Use Strange Dust for training. Default is false."
        );
    }

    public override IEnumerator Execute()
    {
        _ = GetGuardianIndex();
        _ = GetUseStrangeDust();
        yield return Notifications.Engineer;
        yield return Engineer.Claim;
        NextRunTime = Engineer.NextRunTime;
        yield return Engineer.Close;
    }

    private int GetGuardianIndex()
    {
        var value = _guardianIndex?.Value ?? 1;
        if (value >= 0 && value <= 3) return value;

        Debug($"[FAILED] Invalid guardian_index '{value}'. Using default '1'.");
        return 1;
    }

    private bool GetUseStrangeDust()
    {
        if (_useStrangeDust == null)
        {
            Debug("[FAILED] Missing use_strange_dust entry. Using default 'false'.");
            return false;
        }

        return _useStrangeDust.Value;
    }
}