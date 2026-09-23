using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class ReversalPower : ModPowerTemplate
{
    private decimal _pendingDamage;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomIconPath => "res://guestFromHeresy/images/powers/Reversal_temp.png";
    public override string CustomBigIconPath => CustomIconPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Reduction", 0m)];

    public void SetReduction(decimal percent) => DynamicVars["Reduction"].BaseValue = percent;

    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || decimal.Truncate(amount) <= 0m) return amount;
        _pendingDamage = decimal.Truncate(amount) * (1m - DynamicVars["Reduction"].BaseValue / 100m);
        return 0m;
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        // Block has already been spent; the deferred hit must not spend it a second time.
        int turns = Amount;
        decimal damage = _pendingDamage;
        _pendingDamage = 0m;
        await PowerCmd.Remove(this);
        var debt = (ReversalDebtPower)ModelDb.Power<ReversalDebtPower>().ToMutable();
        debt.SetDamage(damage);
        await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), debt, Owner, turns, Owner, null);
    }
}
