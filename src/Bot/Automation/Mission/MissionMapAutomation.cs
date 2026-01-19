using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using FireBot.Config;
using FireBot.Utils;
using UnityEngine;
using static FireBot.Utils.Paths.Missions;
using static FireBot.Utils.StringUtils;

namespace FireBot.Bot.Automation.Mission
{
    internal class MissionMapAutomation : AutomationObserver
    {
        private static List<Mission> _missionCache;

        public override bool ToogleCondition()
        {
            return Buttons.Notification.IsActive();
        }

        public override IEnumerator OnNotificationTriggered()
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

            LogManager.SubHeader("Map Missions");

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
            return new SquadsCountUGUIWrapper().Values.current;
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
                _missionInteractionWrapper?.OnClick();
                yield return new WaitForSeconds(BotSettings.InteractionDelay.Value);
            }
        }

        private class SquadsCountUGUIWrapper : TextMeshProUGUIWrapper
        {
            public SquadsCountUGUIWrapper() : base(SquadsQuantity)
            {
            }

            public (int current, int total) Values => ParseValues();

            private (int current, int total) ParseValues()
            {
                var text = GetParsedText();
                if (string.IsNullOrEmpty(text)) return (0, 0);

                var slashIndex = text.IndexOf('/');
                if (slashIndex == -1) return (0, 0);

                var part1 = text.Substring(0, slashIndex);
                var part2 = text.Substring(slashIndex + 1);

                int.TryParse(part1, out var curr);
                int.TryParse(part2, out var tot);

                return (curr, tot);
            }
        }

        private static class Buttons
        {
            public static readonly ButtonWrapper Start = new ButtonWrapper(StartMissionButton);

            public static readonly ButtonWrapper Notification = new ButtonWrapper(MapMissionNotification);

            public static readonly ButtonWrapper Close = new ButtonWrapper(MissionCloseButton);
        }
    }
}