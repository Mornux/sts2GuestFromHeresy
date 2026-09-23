using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using guestFromHeresy.Keywords;
using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Hooks;
using System.Globalization;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class SensoryExplosion() : SynaestheticAttack(2, CardRarity.Uncommon)
{
    public override TargetType TargetType => Synaesthesia.IsActive(this) ? TargetType.AllEnemies : TargetType.AnyEnemy;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CalculationBaseVar(0m), new ExtraDamageVar(1m), new CalculatedDamageVar(ValueProp.Move).WithMultiplier((_, target) => DebuffStacks(target))];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        string damage = "0";
        if (Synaesthesia.IsActive(this) && CombatState != null)
            damage = string.Join(" / ", CombatState.HittableEnemies.Select(target => decimal.Truncate(
                Hook.ModifyDamage(Owner.RunState, CombatState, target, Owner.Creature, DynamicVars.CalculatedDamage.Calculate(target),
                    ValueProp.Move, this, ModifyDamageHookType.All, CardPreviewMode.Normal, out _)).ToString(CultureInfo.InvariantCulture)));
        description.Add("AllDamage", string.IsNullOrEmpty(damage) ? "0" : damage);
    }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var targets = Synaesthesia.IsActive(this) ? CombatState!.HittableEnemies.ToArray() : new[] { play.Target! };
        foreach (var target in targets.Where(t => t.IsAlive))
        {
            if (Owner.Creature.IsDead) break;
            await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(target)
                .WithHitFx("vfx/vfx_attack_slash").Execute(context);
        }
    }
    protected override void OnUpgrade() => DynamicVars.ExtraDamage.UpgradeValueBy(0.5m);
}
