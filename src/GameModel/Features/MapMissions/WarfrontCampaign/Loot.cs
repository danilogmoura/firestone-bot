using System;
using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Features.MapMissions.WarfrontCampaign;

public class Loot : GameElement
{
    public Loot() : base(Paths.MapMissions.WarfrontCampaign.Loot.Root) { }

    public GameButton ClaimToolsButton => new(Paths.MapMissions.WarfrontCampaign.Loot.ClaimToolsButton, this);

    public DateTime FindNextRunTime =>
        new GameText(Paths.MapMissions.WarfrontCampaign.Loot.NextLootTimeLeft, this).Time;
}