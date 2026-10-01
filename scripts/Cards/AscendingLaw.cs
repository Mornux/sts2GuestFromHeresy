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
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [EnergyHoverTip, HoverTipFactory.FromPower<MegaCrit.Sts2.Core.Models.Powers.DisintegrationPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this);
        foreach (var card in selected)
        {
            if (card.EnergyCost.CostsX) continue;
            int reduction = Math.Max(0, card.EnergyCost.GetWithModifiers(CostModifiers.None));
            card.EnergyCost.SetThisCombat(0);
            if (reduction > 0)
                STS2RitsuLib.Models.Capabilities.ModelCapabilityExtensions.GetOrCreateCapability<AscendingLawCost>(card).Add(reduction);
        }
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}

[RegisterModelCapability]
public sealed class AscendingLawCost : STS2RitsuLib.Models.Capabilities.ModelCapability<MegaCrit.Sts2.Core.Models.CardModel>,
    STS2RitsuLib.Models.Capabilities.IModelCapabilityHookListener, STS2RitsuLib.Models.Capabilities.ICardDescriptionContributor
{
    protected override IEnumerable<MegaCrit.Sts2.Core.Localization.DynamicVars.DynamicVar> CanonicalVars =>
        [new MegaCrit.Sts2.Core.Localization.DynamicVars.DynamicVar("LawDebt", 0m)];
    public void Add(int amount) => Modify(_ => DynamicVars["LawDebt"].BaseValue += amount);
    public IEnumerable<STS2RitsuLib.Models.Capabilities.CardDescriptionFragment> GetDescriptionFragments(STS2RitsuLib.Models.Capabilities.CardDescriptionContext context) =>
        [new(new MegaCrit.Sts2.Core.Localization.LocString("cards", "GUEST_FROM_HERESY_ASCENDING_LAW_AFFIX.description"), STS2RitsuLib.Models.Capabilities.CardDescriptionFragmentPlacement.AfterBase)];
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play) => play.Card == Owner && Owner.Owner.Creature.IsAlive
        ? PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.DisintegrationPower>(context, Owner.Owner.Creature, DynamicVars["LawDebt"].BaseValue, Owner.Owner.Creature, Owner)
        : Task.CompletedTask;
}
