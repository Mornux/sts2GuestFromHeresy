using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class ReversalDebtPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override string CustomIconPath => "res://guestFromHeresy/images/powers/ReversalDebt_temp.png";
    public override string CustomBigIconPath => CustomIconPath;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(0m, ValueProp.Unpowered | ValueProp.Unblockable)];

    public void SetDamage(decimal damage) => DynamicVars.Damage.BaseValue = decimal.Truncate(damage);

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        if (Amount > 1)
        {
            await PowerCmd.Decrement(this);
            return;
        }
        var owner = Owner;
        decimal damage = DynamicVars.Damage.BaseValue;
        await PowerCmd.Remove(this);
        await CreatureCmd.Damage(choiceContext, owner, damage, ValueProp.Unpowered | ValueProp.Unblockable, null, null);
    }
}
