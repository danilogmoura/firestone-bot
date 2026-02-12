using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Features.MapMissions;
using Firebot.GameModel.Features.MapMissions.Missions;
using Firebot.GameModel.Shared;
using Logger = Firebot.Core.Logger;

namespace Firebot.Behaviors.Tasks;

public class MapMissionsTask : BotTask
{
    public override IEnumerator Execute()
    {
        var mainHud = new MainHUD();
        var mapButton = mainHud.MapButton;

        yield return mapButton.Click();

        var allMissions = ScanMissions();
        var toCollect = allMissions.Where(mission => mission.IsCompleted).ToList();
        var toStart = allMissions.Where(mission => !mission.IsActive)
            .OrderByDescending(mission => mission.TimeRequired)
            .ToList();

        foreach (var mission in toCollect)
        {
            yield return mission.OnClick();
            Logger.Debug($"Collecting mission '{mission.Root.name}' with {mission.TimeRequired} remaining.");
        }

        foreach (var mission in toStart)
        {
            yield return mission.OnClick();

            var previewMission = new PreviewMission();
            if (previewMission.IsNotEnoughSquads)
            {
                yield return previewMission.CloseButton.Click();
                break;
            }

            if (previewMission.StartMissionButton.IsVisible())
            {
                yield return previewMission.StartMissionButton.Click();
                Logger.Debug($"Mission '{mission.Root.name}' started.");
            }

            Logger.Debug($"Starting mission '{mission.Root.name}' with {mission.TimeRequired} required.");
        }

        allMissions = ScanMissions();
        var missionHud = new MapMissionHUD();
        NextRunTime = !allMissions.Any() ? missionHud.MissionRefresh.Time() : new ActiveMissions().FindNextRunTime;
        Logger.Debug($"Found {allMissions.Count} missions, next run time in {NextRunTime}");

        yield return missionHud.CloseButton.Click();
    }

    private static List<MissionPin> ScanMissions()
    {
        var missionRoot = new BasePage(Paths.MapMissions.Missions.MapPin.Root);
        var missions = new List<MissionPin>();

        foreach (var gameElement in missionRoot.GetChildren())
        {
            if (!gameElement.IsVisible()) continue;
            foreach (var gameElementChild in gameElement.GetChildren())
            {
                if (!gameElementChild.IsVisible()) continue;
                missions.Add(new MissionPin(gameElementChild.Root));
            }
        }

        return missions;
    }
}