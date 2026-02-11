using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Features.MapMissions;

public class MissionPin : GameElement
{
    public MissionPin(string path, GameElement parent) : base(path, parent) { }

    public GameText TimeRequired => new(Paths.MapMissions.MissionPin.MissionTimeRequirement, this);

    public GameSprite CompletedTick => new(Paths.MapMissions.MissionPin.CompletedTick, this);
}