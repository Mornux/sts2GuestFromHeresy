using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class PsychicBoltPower : TemporaryIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Task BeforeFlush(PlayerChoiceContext context, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player) return Task.CompletedTask;
        int count = Owner.GetPowerAmount<GazedPower>() * Amount;
        foreach (var card in PileType.Hand.GetPile(player).Cards.Take(count).ToArray()) card.GiveSingleTurnRetain();
        return Task.CompletedTask;
    }
}
