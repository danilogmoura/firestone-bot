using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.Core.Tasks;
using Firebot.GameModel.Base;
using Firebot.GameModel.Features.Map;
using Firebot.GameModel.Features.Map.Missions;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using MelonLoader;

namespace Firebot.Behaviors;

public class MapMissionsTask : BotTask
{
    private MelonPreferences_Entry<string> _timeOrder;

    public override string NotificationPath => Paths.BattleLoc.NotificationsLoc.MapMissions;

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

        foreach (var mission in ScanMissions(m => m.IsCompleted))
            yield return mission.Select();

        foreach (var mission in ScanMissions(mission => !mission.IsActive && !mission.IsCompleted, true))
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
        NextRunTime = earliest ?? MapMission.NextRunTime;

        yield return MapMission.Close;
    }

    private IEnumerable<MissionPin> ScanMissions(Func<MissionPin, bool> filter = null, bool sortByTime = false)
    {
        var missionRoot = new GameElement(Paths.MenusLoc.CanvasLoc.MapLoc.MissionsLoc.PinLoc.Root);
        var results = missionRoot.GetChildren().Where(root => root.IsVisible())
            .SelectMany(parent => parent.GetChildren().Where(child => child.IsVisible()))
            .Select(pin => new MissionPin(parent: pin))
            .Where(mission => filter == null || filter(mission))
            .ToList();

        if (sortByTime)
            results = IsAscending()
                ? results.OrderBy(m => m.TimeRequired).ToList()
                : results.OrderByDescending(m => m.TimeRequired).ToList();

        foreach (var mission in results) yield return mission;
    }

    private IEnumerator FindEarliestMissionProgress(Action<DateTime?> setEarliest)
    {
        DateTime? earliest = null;

        foreach (var mission in ScanMissions(mission => mission.IsActive))
        {
            yield return mission.Select();

            var progress = MissionPreview.NextRunTime;
            if (!earliest.HasValue || progress < earliest.Value)
                earliest = progress;

            yield return MissionPreview.Close;
        }

        setEarliest(earliest);
    }

    private bool IsAscending()
    {
        var value = _timeOrder?.Value?.Trim();
        if (string.IsNullOrEmpty(value)) return false;

        if (string.Equals(value, "asc", StringComparison.OrdinalIgnoreCase)) return true;
        if (string.Equals(value, "desc", StringComparison.OrdinalIgnoreCase)) return false;

        Debug($"[FAILED] Invalid mission_time_order '{value}'. Using default 'desc'.");
        return false;
    }
}