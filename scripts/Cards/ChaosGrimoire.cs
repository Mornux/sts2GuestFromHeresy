using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class ChaosGrimoire : ModCardTemplate
{
    private bool _transformPending;
    public ChaosGrimoire() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true) { }
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/ChaosGrimoire.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/ChaosGrimoire.png");
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<Slimed>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        _transformPending = cardPlay.PlayIndex == cardPlay.PlayCount - 1 && !IsDupe;
    }

    public override async Task AfterCardChangedPilesLate(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card != this || !_transformPending || oldPileType != PileType.Play || Pile == null) return;
        _transformPending = false;
        if (!IsTransformable || CombatManager.Instance.IsEnding) return;
        await CardCmd.Transform(this, CardScope!.CreateCard<Slimed>(Owner), CardPreviewStyle.None);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}
