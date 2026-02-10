using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.Core;
using Firebot.Old._Old;
using Firebot.Old._Old.TMProComponents;
using Firebot.Old._Old.UI;
using Firebot.Old._Old.Wrappers;
using Firebot.Old.Automation.Core;
using Firebot.Old.Core;
using UnityEngine;
using static System.Int32;
using static Firebot.Old.Core.BotContext;
using static Firebot.Old.Core.Paths.Missions;
using static Firebot.Utilities.StringUtils;

namespace Firebot.Old.Automation.MapMissions;

internal class MissionMapAutomation : AutomationObserver
{
    private static List<Mission> _missionCache;

    public override bool ShouldExecute() => base.ShouldExecute() && Buttons.Notification.IsActive();

    public override IEnumerator OnNotificationTriggered()
    {
        yield return Buttons.Notification.Click();

        UpdateMissionCache();
        var isClaimAvailable = _missionCache.Any(mission => mission.IsClaim);

        if (GetSquadCount() <= 0 && !isClaimAvailable)
        {
            yield return Buttons.Close.Click();
            yield break;
        }

        foreach (var mission in _missionCache.Where(mission => mission.IsActive && mission.IsClaim))
            yield return mission.Click();

        foreach (var mission in _missionCache.OrderBy(mission => mission.Time)
                     .ToList()
                     .Where(mission => !mission.IsActive && GetSquadCount() > 0))
        {
            yield return mission.Click();
            yield return Buttons.Start.Click();
        }

        yield return Buttons.Close.Click();
    }

    private int GetSquadCount() => new SquadsCountUGUIWrapper().Values.current;

    private void UpdateMissionCache()
    {
        _missionCache = new List<Mission>();

        var missionsContainer = new TransformWrapper(MissionRegion);

        if (!missionsContainer.Exists()) return;

        var children = missionsContainer.GetChildren();

        foreach (var region in children)
        {
            var missions = region.GetChildren();
            foreach (var mission in missions.Where(mission => mission.IsActive()))
            {
                _missionCache.Add(new Mission(region.Name, mission.Name));
                Log.Debug($" Found mission: {region.Name} - {mission.Name}", CorrelationId);
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
            yield return new WaitForSeconds(BotSettings.InteractionDelay);
        }
    }

    private class SquadsCountUGUIWrapper : TextMeshProUGUIWrapper
    {
        public SquadsCountUGUIWrapper() : base(SquadsQuantity) { }

        public (int current, int total) Values => ParseValues();

        private (int current, int total) ParseValues()
        {
            var text = Text;
            if (string.IsNullOrEmpty(text)) return (0, 0);

            var slashIndex = text.IndexOf('/');
            if (slashIndex == -1) return (0, 0);

            var part1 = text.Substring(0, slashIndex);
            var part2 = text.Substring(slashIndex + 1);

            TryParse(part1, out var curr);
            TryParse(part2, out var tot);

            return (curr, tot);
        }
    }

    private static class Buttons
    {
        public static readonly ButtonWrapper Start = new(StartMissionButton);
        public static readonly ButtonWrapper Notification = new(MapMissionNotification);
        public static readonly ButtonWrapper Close = new(MissionCloseButton);
    }
}