using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class Sublimation() : TemporaryArtSkill(0, CardRarity.Uncommon)
{
    protected override string PortraitName => nameof(Sublimation);

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override bool ShouldGlowGoldInternal => guestFromHeresy.Keywords.Synaesthesia.IsActive(this);
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<DisintegrationPower>(),
        STS2RitsuLib.Keywords.ModKeywordRegistry.CreateHoverTip(guestFromHeresy.Keywords.Synaesthesia.Id)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool active = guestFromHeresy.Keywords.Synaesthesia.IsActive(this);
        await PowerCmd.Apply<DisintegrationPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        if (active && IsUpgraded)
        {
            foreach (var card in PileType.Hand.GetPile(Owner).Cards.Where(c => c.IsUpgradable).ToArray()) CardCmd.Upgrade(card);
            return;
        }
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, active ? 2 : 1), card => card.IsUpgradable, this);
        foreach (var card in selected) CardCmd.Upgrade(card);
    }

}
