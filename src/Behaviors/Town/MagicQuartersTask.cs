using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town.MagicQuarters;
using Firebot.GameModel.Primitives;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using MelonLoader;

namespace Firebot.Behaviors.Town;

public class MagicQuartersTask : BotTask
{
    private MelonPreferences_Entry<int> _guardianIndex;
    private MelonPreferences_Entry<bool> _useStrangeDust;

    public override string NotificationPath => Paths.BattleLoc.NotificationsLoc.GuardianTrainingBtn;

    public override IEnumerator Execute()
    {
        yield return Notifications.GuardianTraining;

        var preferredIndex = GetGuardianIndex();
        var useStrangeDust = GetUseStrangeDust();
        var trainBtn = MagicQuarters.TrainBtn;
        var guardianIndex = FindAvailableGuardian(preferredIndex);

        if (guardianIndex >= 0)
        {
            var child = MagicQuarters.Guardians.GetChild(guardianIndex);
            var guardianBtn = new GameButton(parent: child);
            yield return TryTrainGuardian(guardianBtn, trainBtn, useStrangeDust);
        }

        NextRunTime = MagicQuarters.NextRunTime;
        yield return MagicQuarters.CloseLockedPopup;
        yield return MagicQuarters.Close;
    }

    protected override void OnConfigure(MelonPreferences_Category category)
    {
        if (_guardianIndex != null) return;

        _guardianIndex = category.CreateEntry(
            "guardian_index",
            0,
            "Guardian Index",
            "Select guardian index for training. Use 0-3. Default is 0.\n" +
            "0=Vermilion, 1=Grace, 2=Ankaa, 3=Azhar"
        );

        _useStrangeDust = category.CreateEntry(
            "use_strange_dust",
            false,
            "Use Strange Dust",
            "Use 'Strange Dust' for training. Default is false."
        );
    }

    private int FindAvailableGuardian(int preferredIndex)
    {
        var result = TryGuardianAtIndex(preferredIndex);
        if (result >= 0) return result;

        for (var i = 0; i <= 3; i++)
        {
            if (i == preferredIndex) continue;

            result = TryGuardianAtIndex(i);
            if (result >= 0) return result;
        }

        return 0;
    }

    private int TryGuardianAtIndex(int index)
    {
        var child = MagicQuarters.Guardians.GetChild(index);

        var guardian = new Guardian(parent: child);
        if (guardian.IsVisible() && guardian.IsUnlocked)
            return index;

        return 0;
    }

    private IEnumerator TryTrainGuardian(GameButton guardianBtn, GameButton trainBtn, bool useStrangeDust)
    {
        yield return guardianBtn.Click();
        yield return trainBtn.Click();
        yield return ApplyEnlightenment(useStrangeDust);
    }

    private IEnumerator ApplyEnlightenment(bool useStrangeDust)
    {
        if (!useStrangeDust) yield break;

        var enlightenmentBtn = Guardian.EnlightenmentBtn;
        while (enlightenmentBtn.IsClickable()) yield return enlightenmentBtn.Click();
    }


    private int GetGuardianIndex()
    {
        var value = _guardianIndex?.Value ?? 0;

        if (value >= 0 && value <= 3) return value;

        Debug($"[FAILED] Invalid guardian_index '{value}'. Using default '0'.");
        return 0;
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