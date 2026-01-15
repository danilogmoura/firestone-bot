using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FireBot.Bot.Component;
using FireBot.Utils;
using UnityEngine;
using static FireBot.Utils.BotConstants;
using static FireBot.Utils.Paths.Missions;
using static FireBot.Utils.StringUtils;

namespace FireBot.Bot.Automation.Mission
{
    internal static class MissionMapAutomation
    {
        private static List<Mission> _missionCache;

        public static IEnumerator Process()
        {
            if (!Buttons.Notification.IsActive()) yield break;

            yield return Buttons.Notification.Click();

            UpdateMissionCache();
            var isClaimAvailable = _missionCache.Any(mission => mission.IsClaim);

            if (GetSquadCount() <= 0 && !isClaimAvailable)
            {
                yield return Buttons.Close.Click();
                yield break;
            }

            LogManager.SubHeader("Missions");

            foreach (var mission in _missionCache.Where(mission => mission.IsActive && mission.IsClaim))
                yield return mission.Click();

            foreach (var mission in _missionCache.OrderByDescending(mission => mission.Time)
                         .ToList()
                         .Where(mission => !mission.IsActive && GetSquadCount() > 0))
            {
                yield return mission.Click();
                yield return Buttons.Start.Click();
            }

            yield return Buttons.Close.Click();
        }

        private static int GetSquadCount()
        {
            return new SquadsCountUGUIWrapper().Current;
        }

        private static void UpdateMissionCache()
        {
            _missionCache = new List<Mission>();

            var missionsContainer = new ObjectWrapper(MissionRegion);
            var regionsRoot = missionsContainer.Transform;

            if (regionsRoot == null) return;

            for (var i = 0; i < regionsRoot.childCount; i++)
            {
                var region = regionsRoot.GetChild(i);

                for (var j = 0; j < region.childCount; j++)
                {
                    var mission = region.GetChild(j);
                    if (!mission.gameObject.activeInHierarchy) continue;

                    _missionCache.Add(new Mission(region.name, mission.name));
                }
            }
        }

        private readonly struct Mission
        {
            private readonly SpriteRendererWrapper _activeIconWrapper;
            private readonly SpriteRendererWrapper _completedTickWrapper;
            private readonly MapMissionInteractionWrapper _missionInteractionWrapper;

            public readonly string Name;
            public readonly string Region;
            public readonly int Time;

            public Mission(string missionRegion, string missionName)
            {
                Region = missionRegion;
                Name = missionName;

                _activeIconWrapper =
                    new SpriteRendererWrapper(JoinPath(MissionRegion, Region, Name, "missionActiveIcon"));

                _completedTickWrapper =
                    new SpriteRendererWrapper(JoinPath(MissionRegion, Region, Name, "missionBg/completedTick"));

                _missionInteractionWrapper = new MapMissionInteractionWrapper(JoinPath(MissionRegion, Region, Name));

                Time = new TimeDisplay(JoinPath(MissionRegion, Region, Name, "missionBg/missionTimeBg/missionTimeReq"))
                    .ParseToSeconds();
            }

            public bool IsActive => _activeIconWrapper.Enabled();
            public bool IsClaim => _completedTickWrapper.Enabled();

            public IEnumerator Click()
            {
                _missionInteractionWrapper.OnClick();
                yield return new WaitForSeconds(InteractionDelay);
            }
        }

        private class SquadsCountUGUIWrapper : TextMeshProUGUIWrapper
        {
            public SquadsCountUGUIWrapper() : base(SquadsQuantity)
            {
            }

            public int Current => ParseValues().current;

            public int Total => ParseValues().total;

            private (int current, int total) ParseValues()
            {
                var text = GetParsedText();

                if (string.IsNullOrWhiteSpace(text)) return (0, 0);

                var parts = text.Trim().Split('/');

                if (parts.Length != 2) return (0, 0);

                int.TryParse(parts[0], out var curr);
                int.TryParse(parts[1], out var tot);

                return (curr, tot);
            }
        }

        private static class Buttons
        {
            public static ButtonWrapper Start => new ButtonWrapper(StartMissionButton);
            public static ButtonWrapper Notification => new ButtonWrapper(GridMissionButton);
            public static ButtonWrapper Close => new ButtonWrapper(MissionCloseButton);
        }
    }
}