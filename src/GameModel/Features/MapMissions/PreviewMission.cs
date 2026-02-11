using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Features.MapMissions;

public class PreviewMission : GameElement
{
    public PreviewMission() : base(Paths.MapMissions.PreviewMission.Root) { }

    public GameButton CloseButton => new(Paths.MapMissions.PreviewMission.CloseButton, this);

    public SpeedUpButton SpeedUpButton => new(this);

    public GameButton StartMissionButton => new(Paths.MapMissions.PreviewMission.StartMissionButton, this);
}