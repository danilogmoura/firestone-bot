using System;
using System.Collections;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using Il2Cpp;
using UnityEngine;
using UnityEngine.EventSystems;
using static Firebot.Core.BotSettings;

namespace Firebot.GameModel.Features.Map.Missions;

public class MissionPin : GameElement
{
    public MissionPin(string path = null, GameElement parent = null, Transform transform = null)
        : base(path, parent, transform) { }

    public DateTime TimeRequired =>
        new GameText(Paths.MenusLoc.CanvasLoc.MapLoc.MissionsLoc.PinLoc.TimeReq, this).Time;

    public bool IsActive =>
        new GameElement(Paths.MenusLoc.CanvasLoc.MapLoc.MissionsLoc.PinLoc.ActiveIcon, this).IsVisible();

    public bool IsCompleted =>
        new GameElement(Paths.MenusLoc.CanvasLoc.MapLoc.MissionsLoc.PinLoc.Tick, this).IsVisible();

    public IEnumerator Select()
    {
        if (!IsVisible())
        {
            Debug($"[FAILED] OnClick ignored: MissionPin not visible. Path: {Path}");
            yield return new WaitForSeconds(InteractionDelay);
            yield break;
        }

        if (TryGetComponent(out MapMissionInteraction interaction))
            try
            {
                var fakeEvent = new PointerEventData(EventSystem.current)
                {
                    button = PointerEventData.InputButton.Left
                };
                interaction.OnPointerClick(fakeEvent);
            }
            catch (Exception e)
            {
                Debug($"[ERROR] Exception during Custom Click: {e.Message}. Path: {Path}");
            }
        else
            Debug($"[FAILED] MissionPin found but MapMissionInteraction component is missing. Path: {Path}");

        yield return new WaitForSeconds(InteractionDelay);
    }
}