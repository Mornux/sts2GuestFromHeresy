using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class GreatBloodSacrifice : ModCardTemplate
{
    public GreatBloodSacrifice() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self, true) { }
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/GreatBloodSacrifice.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/GreatBloodSacrifice.png");
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(4), new CardsVar(4)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [EnergyHoverTip, HoverTipFactory.FromPower<GreatBloodSacrificePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await PowerCmd.Apply<GreatBloodSacrificePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
