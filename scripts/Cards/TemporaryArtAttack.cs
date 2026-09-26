using guestFromHeresy.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

public abstract class TemporaryArtAttack(int cost, CardRarity rarity, TargetType target = TargetType.AnyEnemy)
    : ModCardTemplate(cost, CardType.Attack, rarity, target, true)
{
    protected virtual string PortraitName => "TempImage";
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://guestFromHeresy/images/cards/{PortraitName}.png",
        BetaPortraitPath: $"res://guestFromHeresy/images/cards/{PortraitName}.png");

    protected AttackCommand Attack(decimal damage, CardPlay play, bool all = false, int hits = 1)
    {
        var attack = DamageCmd.Attack(damage).FromCard(this).WithHitCount(hits);
        if (all) attack.TargetingAllOpponents(CombatState!);
        else attack.Targeting(play.Target ?? throw new InvalidOperationException("An enemy target is required."));
        return attack.WithHitFx("vfx/vfx_attack_slash");
    }

    internal static int NegativeStacks(PowerModel power) => power.Type == PowerType.Debuff
        ? Math.Max(0, power.Amount) : power.AllowNegative ? Math.Max(0, -power.Amount) : 0;
    internal static int DebuffKinds(Creature? target) => target?.Powers.Where(p => NegativeStacks(p) > 0)
        .Select(p => p.Id).Distinct().Count() ?? 0;
    internal static int DebuffStacks(Creature? target) => target?.Powers.Sum(NegativeStacks) ?? 0;
}

public abstract class SynaestheticAttack(int cost, CardRarity rarity, TargetType target = TargetType.AnyEnemy)
    : TemporaryArtAttack(cost, rarity, target)
{
    protected override bool ShouldGlowGoldInternal => Synaesthesia.IsActive(this);
}
