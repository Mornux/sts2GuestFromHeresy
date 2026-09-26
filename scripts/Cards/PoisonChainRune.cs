using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class PoisonChainRune() : TemporaryArtSkill(1, CardRarity.Rare)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.Static(StaticHoverTip.Stun)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => PowerCmd.Apply<PoisonChainRunePower>(context, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
