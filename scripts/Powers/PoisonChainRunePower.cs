using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class PoisonChainRunePower : TemporaryIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext context, PowerModel power, decimal amount, Creature? applier, CardModel? source)
    {
        if (power.Owner != Owner || EternalPlanePower.AppliedDebuffLayers(power, amount) <= 0 ||
            applier?.Monster == null || applier.Side == Owner.Side) return;
        await PowerCmd.Remove(this);
        if (applier.IsAlive) await CreatureCmd.Stun(applier);
    }
}
