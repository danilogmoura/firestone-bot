using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.Core.Tasks;
using Firebot.GameModel.Base;
using Firebot.GameModel.Features.MapMissions;
using Firebot.GameModel.Features.MapMissions.Missions;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using MelonLoader;

namespace Firebot.Behaviors;

public class MapMissionsTask : BotTask
{
    private MelonPreferences_Entry<string> _timeOrder;

    protected override void OnConfigure(MelonPreferences_Category category)
    {
        if (_timeOrder != null) return;

        _timeOrder = category.CreateEntry(
            "mission_time_order",
            "desc",
            "Mission Time Order",
            "Sort missions by time required. Use 'asc' (shorter first) or 'desc' (longer first)."
        );
    }

    public override IEnumerator Execute()
    {
        yield return Notifications.MapMissions;

        var toCollect = ScanMissions().Where(m => m.IsCompleted).ToList();
        foreach (var mission in toCollect)
            yield return mission.Select();

        var pending = ScanMissions().Where(mission => !mission.IsActive && !mission.IsCompleted);
        var toStart = IsAscending()
            ? pending.OrderBy(mission => mission.TimeRequired).ToList()
            : pending.OrderByDescending(m => m.TimeRequired).ToList();
        foreach (var mission in toStart)
        {
            yield return mission.Select();

            if (MissionPreview.IsNotEnoughSquads)
            {
                yield return MissionPreview.Close;
                break;
            }

            yield return MissionPreview.StartMission;
        }

        DateTime? earliest = null;
        yield return FindEarliestMissionProgress(value => earliest = value);
        NextRunTime = earliest ?? MapMissionHUD.MissionRefresh.Time;

        yield return MapMissionHUD.CloseButton.Click();
    }

    private IEnumerable<MissionPin> ScanMissions()
    {
        Debug("[SCAN] Starting Mission Pin discovery...");
        var missionRoot = new GameElement(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.MissionsLoc.PinLoc.Root);

        foreach (var category in missionRoot.GetChildren())
        {
            if (!category.IsVisible()) continue;
            foreach (var pinElement in category.GetChildren())
            {
                if (!pinElement.IsVisible()) continue;
                yield return new MissionPin(parent: pinElement);
            }
        }
    }

    private IEnumerator FindEarliestMissionProgress(Action<DateTime?> setEarliest)
    {
        DateTime? earliest = null;

        foreach (var mission in ScanMissions().Where(mission => mission.IsActive))
        {
            yield return mission.Select();

            var progress = MissionPreview.MissionProgress;
            if (!earliest.HasValue || progress < earliest.Value)
                earliest = progress;

            yield return MissionPreview.Close;
        }

        setEarliest(earliest);
    }

    private bool IsAscending()
        => string.Equals(_timeOrder?.Value?.Trim(), "asc", StringComparison.OrdinalIgnoreCase);
}