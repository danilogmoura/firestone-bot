using System;
using System.Collections;
using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;
using Il2Cpp;
using UnityEngine;
using UnityEngine.EventSystems;
using static Firebot.Core.BotSettings;

namespace Firebot.GameModel.Features.MapMissions.Missions;

public class MissionPin : GameElement
{
    public MissionPin(string path, GameElement parent) : base(path, parent) { }

    public MissionPin(Transform root, string path = null) : base(root, path) { }

    public DateTime TimeRequired => new GameText(Paths.MapMissions.Missions.MapPin.MissionTimeRequirement, this).Time();

    public bool IsActive =>
        IsVisible() && new GameSprite(Paths.MapMissions.Missions.MapPin.MissionActiveIcon, this).IsVisible();

    public bool IsCompleted =>
        IsVisible() && new GameSprite(Paths.MapMissions.Missions.MapPin.CompletedTick, this).IsVisible();

    public IEnumerator OnClick()
    {
        if (CachedTransform == null) yield break;

        var fakeEvent = new PointerEventData(EventSystem.current)
        {
            button = PointerEventData.InputButton.Left
        };
        CachedTransform.TryGetComponent(out MapMissionInteraction group);
        group.OnPointerClick(fakeEvent);
        yield return new WaitForSeconds(InteractionDelay);
    }
}