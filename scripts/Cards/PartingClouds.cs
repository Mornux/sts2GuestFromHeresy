using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class PartingClouds() : TemporaryArtAttack(2, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("Hits", IsMutable ? DebuffStacks(Owner.Creature) : 0);
    }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var debuffs = Owner.Creature.Powers.Where(p => p.Type == PowerType.Debuff || NegativeStacks(p) > 0).ToArray();
        int removed = 0;
        foreach (var power in debuffs)
        {
            int before = NegativeStacks(power);
            await PowerCmd.Remove(power);
            if (!Owner.Creature.Powers.Contains(power)) removed += before;
        }
        for (int i = 0; i < removed && Owner.Creature.IsAlive && !CombatManager.Instance.IsOverOrEnding; i++)
        {
            var enemies = CombatState!.HittableEnemies.Where(c => c.IsAlive).ToArray();
            if (enemies.Length == 0) break;
            var target = Owner.RunState.Rng.CombatTargets.NextItem(enemies)!;
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(target)
                .WithHitFx("vfx/vfx_attack_slash").Execute(context);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
