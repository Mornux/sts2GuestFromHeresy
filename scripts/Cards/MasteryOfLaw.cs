using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class MasteryOfLaw() : TemporaryArtSkill(1, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override string PortraitName => nameof(MasteryOfLaw);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HpLossVar(2m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not { } target) return;
        int repeats = 1 + CombatManager.Instance.History.Entries.OfType<PowerReceivedEntry>()
            .Where(entry => entry.Applier == Owner.Creature && entry.Amount > 0)
            .Select(entry => entry.Power.Id).Distinct().Count();
        for (int i = 0; i < repeats && !target.IsDead && !Owner.Creature.IsDead; i++)
            await CreatureCmd.Damage(choiceContext, target, DynamicVars.HpLoss.BaseValue,
                ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this, cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars.HpLoss.UpgradeValueBy(1m);
}
