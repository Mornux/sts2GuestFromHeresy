using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class DelusionMadeRealPower : TemporaryIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext context, PowerModel power, decimal amount, Creature? applier, CardModel? source)
    {
        if (applier != Owner || power is not DisintegrationPower || amount <= 0) return;
        await CreatureCmd.GainBlock(Owner, Amount, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered, null);
    }
}
