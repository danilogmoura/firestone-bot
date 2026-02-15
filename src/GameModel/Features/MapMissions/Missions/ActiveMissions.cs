using System;
using Firebot.Core;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Features.MapMissions.Missions;

public class ActiveMissions : GameElement
{
    public ActiveMissions() : base(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.MissionsLoc.ActiveGrid) { }

    public DateTime FindNextRunTime
    {
        get
        {
            var earliestDate = DateTime.MaxValue;
            var foundAny = false;

            foreach (var gameElement in GetChildren())
            {
                if (!gameElement.IsVisible()) continue;

                var timer =
                    new GameText(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.MissionsLoc.ProgressSuffix, gameElement);

                var missionFinishTime = timer.Time;

                if (missionFinishTime >= earliestDate || missionFinishTime <= DateTime.Now) continue;

                earliestDate = missionFinishTime;
                foundAny = true;
            }

            return foundAny ? earliestDate : DateTime.Now.AddMinutes(1);
        }
    }
}