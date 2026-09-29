using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class FarewellLight() : TemporaryArtSkill(1, CardRarity.Uncommon)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromCard<ShadowShield>(IsUpgraded)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var hand = PileType.Hand.GetPile(Owner).Cards;
        int count = CardPile.MaxCardsInHand - hand.Count;
        for (int i = 0; i < count; i++)
        {
            var shield = CardScope!.CreateCard<ShadowShield>(Owner);
            if (IsUpgraded) CardCmd.Upgrade(shield, MegaCrit.Sts2.Core.Nodes.CommonUi.CardPreviewStyle.None);
            await CardPileCmd.AddGeneratedCardToCombat(shield, PileType.Hand, Owner);
        }
    }
}
