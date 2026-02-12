using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Features.MapMissions;
using Firebot.GameModel.Features.MapMissions.Missions;
using Firebot.GameModel.Shared;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.Behaviors.Tasks;

public class MapMissionsTask : BotTask
{
    public override IEnumerator Execute()
    {
        var mainHud = new MainHUD();
        var mapButton = mainHud.MapButton;

        if (!mapButton.IsClickable()) yield break;

        mapButton.Click();
        yield return new WaitForSeconds(1f);

        var allMissions = ScanMissions();
        var toCollect = allMissions.Where(mission => mission.IsCompleted).ToList();
        var toStart = allMissions.Where(mission => !mission.IsActive).OrderByDescending(m => m.TimeRequired).ToList();

        foreach (var mission in toCollect)
        {
            mission.OnClick();
            yield return new WaitForSeconds(1f);
            Logger.Debug($"Collecting mission '{mission.Root.name}' with {mission.TimeRequired} remaining.");
        }

        foreach (var mission in toStart)
        {
            mission.OnClick();
            yield return new WaitForSeconds(1f);

            var previewMission = new PreviewMission();
            if (previewMission.IsNotEnoughSquads)
            {
                previewMission.CloseButton.Click();
                yield return new WaitForSeconds(1f);
                break;
            }

            if (previewMission.StartMissionButton.IsVisible())
            {
                previewMission.StartMissionButton.Click();
                Logger.Debug($"Mission '{mission.Root.name}' started.");
            }

            yield return new WaitForSeconds(1f);
            Logger.Debug($"Starting mission '{mission.Root.name}' with {mission.TimeRequired} required.");
        }

        allMissions = ScanMissions();
        var missionHud = new MapMissionHUD();
        NextRunTime = !allMissions.Any() ? missionHud.MissionRefresh.Time() : new ActiveMissions().FindNextRunTime;
        Logger.Debug($"Found {allMissions.Count} missions, next run time in {NextRunTime}");

        missionHud.CloseButton.Click();
        yield return new WaitForSeconds(1f);
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