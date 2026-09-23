using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class GravityMomentPower : ModPowerTemplate
{
    private List<AttackCommand> _attacks = [];
    private AttackCommand? _redirectedAttack;
    private decimal? _pendingDamage;
    private bool _redirecting;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string CustomIconPath => "res://guestFromHeresy/images/powers/GravityMoment_temp.png";
    public override string CustomBigIconPath => CustomIconPath;

    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        _attacks = [];
        _redirectedAttack = null;
        _pendingDamage = null;
        _redirecting = false;
    }

    public override Task BeforeAttack(AttackCommand command)
    {
        _attacks.Add(command);
        return Task.CompletedTask;
    }

    // Called only by the actual damage pipeline, after modifiers but before spending Block.
    public decimal Redirect(decimal damage, ValueProp props, Creature? dealer)
    {
        if (_redirecting || !props.IsPoweredAttack() || dealer == Owner) return damage;
        var current = _attacks.LastOrDefault();
        if (_redirectedAttack != null && current != _redirectedAttack) return damage;
        _redirectedAttack = current;
        _pendingDamage = decimal.Truncate(damage);
        return 0m;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext context, Creature target, DamageResult result,
        ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || _pendingDamage is not { } damage) return;
        _pendingDamage = null;
        if (_redirectedAttack == null) await PowerCmd.Remove(this);
        var enemies = Owner.CombatState?.HittableEnemies.ToArray() ?? [];
        _redirecting = true;
        try
        {
            foreach (var enemy in enemies.Where(e => e.IsAlive))
                await CreatureCmd.Damage(context, enemy, damage, ValueProp.Unpowered, Owner, null);
        }
        finally { _redirecting = false; }
    }

    public override async Task AfterAttack(PlayerChoiceContext context, AttackCommand command)
    {
        _attacks.Remove(command);
        if (command == _redirectedAttack) await PowerCmd.Remove(this);
    }
}
