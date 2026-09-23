using guestFromHeresy.Keywords;
using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class AscendingLaw() : TemporaryArtSkill(0, CardRarity.Uncommon)
{
    protected override string PortraitName => nameof(AscendingLaw);

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override bool ShouldGlowGoldInternal => Synaesthesia.IsActive(this);
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [EnergyHoverTip, HoverTipFactory.Static(StaticHoverTip.ReplayStatic), ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool active = Synaesthesia.IsActive(this);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this);
        foreach (var card in selected)
        {
            if (!card.EnergyCost.CostsX && card.EnergyCost.GetWithModifiers(CostModifiers.None) >= 0)
                card.EnergyCost.AddUntilPlayed(-1);
            if (active) card.BaseReplayCount++;
        }
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
