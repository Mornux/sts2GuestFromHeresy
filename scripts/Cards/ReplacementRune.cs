using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class ReplacementRune() : TemporaryArtSkill(1, CardRarity.Uncommon)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromCard<ShadowMissile>(), HoverTipFactory.Static(StaticHoverTip.Transform)];

    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => PowerCmd.Apply<ReplacementRunePower>(context, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() { EnergyCost.UpgradeBy(-1); }
}
