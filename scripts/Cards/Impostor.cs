using MegaCrit.Sts2.Core.HoverTips;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class Impostor() : TemporaryArtPower(3, CardRarity.Rare)
{
    protected override string PortraitName => nameof(Impostor);
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<Assimilation>()];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var cards = PileType.Hand.GetPile(Owner).Cards.ToArray();
        foreach (var card in cards) await CardCmd.Exhaust(context, card);
        for (int i = 0; i < cards.Length; i++)
            await CardPileCmd.AddGeneratedCardToCombat(CardScope!.CreateCard<Assimilation>(Owner), PileType.Hand, Owner);
    }
    protected override void OnUpgrade() { EnergyCost.UpgradeBy(-1); AddKeyword(CardKeyword.Retain); }
}
