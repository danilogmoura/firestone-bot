using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.Core;
using Firebot.GameModel.Base;
using Firebot.GameModel.Features.MapMissions;
using Firebot.GameModel.Features.MapMissions.Missions;
using Firebot.GameModel.Shared;
using MelonLoader;

namespace Firebot.Behaviors.Tasks;

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
        yield return MainHUD.MapButton.Click();
        var allMissions = ScanMissions();

        var toCollect = allMissions.Where(m => m.IsCompleted).ToList();
        foreach (var mission in toCollect)
        {
            yield return mission.OnClick();
            Debug($"[TASK] Collected: {mission.Name}");
        }

        var pending = allMissions.Where(m => !m.IsActive && !m.IsCompleted);
        var toStart = IsAscending()
            ? pending.OrderBy(m => m.TimeRequired).ToList()
            : pending.OrderByDescending(m => m.TimeRequired).ToList();
        foreach (var mission in toStart)
        {
            yield return mission.OnClick();

            if (PreviewMission.IsNotEnoughSquads)
            {
                Debug("[TASK] Stopping: No more squads available.");
                yield return PreviewMission.CloseButton.Click();
                break;
            }

            yield return PreviewMission.StartMissionButton.Click();
            Debug($"[TASK] Started mission: {mission.Name}");
        }

        var activeMissions = new ActiveMissions();
        if (allMissions.Any(m => m.IsActive))
            NextRunTime = activeMissions.FindNextRunTime;
        else
            NextRunTime = MapMissionHUD.MissionRefresh.Time;

        yield return MapMissionHUD.CloseButton.Click();
    }

    private List<MissionPin> ScanMissions()
    {
        Debug("[SCAN] Starting Mission Pin discovery...");

        var missionRoot = new GameElement(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.MissionsLoc.PinLoc.Root);
        var missions = new List<MissionPin>();

        foreach (var category in missionRoot.GetChildren())
        {
            if (!category.IsVisible()) continue;

            foreach (var pinElement in category.GetChildren())
            {
                if (!pinElement.IsVisible()) continue;

                var missionPin = new MissionPin(parent: pinElement);
                missions.Add(missionPin);
            }
        }

        Debug($"[SCAN] Completed. Found {missions.Count} active missions.");
        return missions;
    }

    private bool IsAscending()
        => string.Equals(_timeOrder?.Value?.Trim(), "asc", StringComparison.OrdinalIgnoreCase);
}