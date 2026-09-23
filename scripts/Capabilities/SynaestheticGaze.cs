using guestFromHeresy.Keywords;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Models.Capabilities;

namespace guestFromHeresy.Capabilities;

[RegisterModelCapability]
public sealed class SynaestheticGaze : ModelCapability<CardModel>, IModelCapabilityHookListener,
    ICardDescriptionContributor, ICardHoverTipContributor, ICardGlowContributor
{
    private HashSet<CardPlay> _activePlays = [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("GlimpseGazed", 0m)];
    public void AddStack() => Modify(_ => DynamicVars["GlimpseGazed"].BaseValue++);

    public IEnumerable<CardDescriptionFragment> GetDescriptionFragments(CardDescriptionContext context) =>
        [new(new LocString("cards", "GUEST_FROM_HERESY_GLIMPSE_AFFIX.description"), CardDescriptionFragmentPlacement.AfterBase)];
    public IEnumerable<IHoverTip> GetHoverTips(CardModel card) =>
        [HoverTipFactory.FromPower<GazedPower>(), ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];
    public bool ShouldGlowGold(CardModel card) => Synaesthesia.IsActive(card);

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card == Owner && Synaesthesia.IsActive(Owner)) _activePlays.Add(cardPlay);
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card != Owner || !_activePlays.Remove(cardPlay)) return;
        await PowerCmd.Apply<GazedPower>(choiceContext, Owner.Owner.Creature,
            DynamicVars["GlimpseGazed"].BaseValue, Owner.Owner.Creature, Owner);
    }

    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        // Transient play state is deliberately not carried into copies or previews.
        _activePlays = [];
    }
}
