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
public sealed class WhatTheCloudsConcealed() : TemporaryArtAttack(0, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override string PortraitName => nameof(WhatTheCloudsConcealed);
    protected override bool HasEnergyCostX => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        int x = IsMutable && Owner.PlayerCombatState != null && CombatState is { } combat
            ? MegaCrit.Sts2.Core.Hooks.Hook.ModifyXValue(combat, this,
                Pile?.Type == PileType.Play ? EnergyCost.CapturedXValue : Owner.PlayerCombatState.Energy) : 0;
        int layers = Math.Max(0, x + (IsUpgraded ? 1 : 0));
        description.Add("Hits", IsMutable ? Owner.Creature.Powers.Sum(p => Math.Min(NegativeStacks(p), layers)) : 0);
    }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var debuffs = Owner.Creature.Powers.Where(p => p.Type == PowerType.Debuff || NegativeStacks(p) > 0).ToArray();
        int removed = 0;
        int layers = Math.Max(0, ResolveEnergyXValue() + (IsUpgraded ? 1 : 0));
        foreach (var power in debuffs)
        {
            int before = NegativeStacks(power);
            int reduction = Math.Min(before, layers);
            if (reduction == 0) continue;
            await PowerCmd.ModifyAmount(context, power, power.Amount < 0 ? reduction : -reduction, Owner.Creature, this);
            int after = Owner.Creature.Powers.Contains(power) ? NegativeStacks(power) : 0;
            removed += Math.Max(0, before - after);
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
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); AddKeyword(CardKeyword.Retain); }
}
