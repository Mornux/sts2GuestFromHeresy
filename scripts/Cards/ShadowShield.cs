using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class ShadowShield : ModCardTemplate
{
    public ShadowShield() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/ShadowShield.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/ShadowShield.png");
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<DisintegrationPower>()];

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

    public override async Task AfterCardChangedPilesLate(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card != this || Pile?.Type != PileType.Discard || oldPileType == PileType.Discard) return;
        await PowerCmd.Apply<DisintegrationPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature, 2m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2m);
}
