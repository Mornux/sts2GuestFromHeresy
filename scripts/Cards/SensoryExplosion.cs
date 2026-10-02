using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class SensoryExplosion() : TemporaryArtSkill(2, CardRarity.Uncommon)
{
    protected override string PortraitName => nameof(SensoryExplosion);
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("LayersPerTrigger", 2m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<DisintegrationPower>()];
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        int layers = IsMutable ? Owner.Creature.GetPowerAmount<DisintegrationPower>() : 0;
        description.Add("Triggers", layers / DynamicVars["LayersPerTrigger"].IntValue);
    }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var power = Owner.Creature.GetPower<DisintegrationPower>();
        if (power == null) return;
        int triggers = power.Amount / DynamicVars["LayersPerTrigger"].IntValue;
        await PowerCmd.Remove(power);
        for (int i = 0; i < triggers && Owner.Creature.IsAlive; i++)
            foreach (var enemy in CombatState!.HittableEnemies.ToArray())
                if (enemy.IsAlive && enemy.GetPower<DisintegrationPower>() is { } enemyPower)
                    await DisintegrationTriggers.Trigger(context, enemyPower);
    }
    protected override void OnUpgrade() => DynamicVars["LayersPerTrigger"].UpgradeValueBy(-1m);
}
