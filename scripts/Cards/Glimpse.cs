using guestFromHeresy.Capabilities;
using guestFromHeresy.Keywords;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Models.Capabilities;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class Glimpse() : TemporaryArtSkill(1, CardRarity.Common)
{
    protected override string PortraitName => nameof(Glimpse);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<GazedPower>(1m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<GazedPower>(), ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<GazedPower>(choiceContext, Owner.Creature,
            DynamicVars["GazedPower"].BaseValue, Owner.Creature, this);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this);
        foreach (var card in selected)
            card.GetOrCreateCapability<SynaestheticGaze>().AddStack();
    }

    protected override void OnUpgrade() => DynamicVars["GazedPower"].UpgradeValueBy(1m);
}
