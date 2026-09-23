using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class ResentmentBurst() : TemporaryArtSkill(2, CardRarity.Rare)
{
    protected override string PortraitName => nameof(ResentmentBurst);

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<DisintegrationPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combat = Owner.Creature.CombatState;
        if (combat == null) return;
        for (int i = 0; i < 2; i++)
        {
            foreach (var creature in combat.Creatures.ToArray())
            {
                if (creature.IsDead || creature.GetPower<DisintegrationPower>() is not { Amount: > 0 } power) continue;
                await CreatureCmd.Damage(choiceContext, creature, power.Amount, ValueProp.Unpowered, creature, null);
            }
        }
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
