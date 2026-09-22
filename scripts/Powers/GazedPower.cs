using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class GazedPower : ModPowerTemplate
{
    private int _strength;
    private int _dexterity;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomIconPath => "res://guestFromHeresy/images/projection/gazed.svg";
    public override string CustomBigIconPath => CustomIconPath;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        int bonus = ProjectionManager.Readings(Owner.Player!);
        if (bonus <= 0) return;
        var context = new ThrowingPlayerChoiceContext();
        int before = Owner.GetPowerAmount<StrengthPower>();
        await PowerCmd.Apply<StrengthPower>(context, Owner, bonus, Owner, null);
        _strength = Owner.GetPowerAmount<StrengthPower>() - before;
        before = Owner.GetPowerAmount<DexterityPower>();
        await PowerCmd.Apply<DexterityPower>(context, Owner, bonus, Owner, null);
        _dexterity = Owner.GetPowerAmount<DexterityPower>() - before;
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> creatures)
    {
        // A duration includes the enemy turn, so protection does not expire before attacks.
        return side == CombatSide.Enemy ? PowerCmd.Decrement(this) : Task.CompletedTask;
    }

    public void RemoveProjectionBonuses()
    {
        RemoveBonus<StrengthPower>(_strength);
        RemoveBonus<DexterityPower>(_dexterity);
        _strength = _dexterity = 0;
    }

    private void RemoveBonus<T>(int amount) where T : PowerModel
    {
        if (amount == 0) return;
        PowerModel? power = Owner.GetPower<T>();
        if (power == null)
        {
            // The combined stat may have reached zero while the temporary bonus was active.
            power = ModelDb.Power<T>().ToMutable();
            power.ApplyInternal(Owner, -amount, true);
        }
        else
        {
            power.SetAmount(power.Amount - amount, true);
            if (power.ShouldRemoveDueToAmount()) power.RemoveInternal();
        }
    }

    public override Task AfterRemoved(Creature owner)
    {
        RemoveProjectionBonuses();
        return Task.CompletedTask;
    }
}
