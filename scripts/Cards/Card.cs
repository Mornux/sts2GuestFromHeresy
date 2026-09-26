using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
[RegisterCharacterStarterCard(typeof(Characters.Character), 3)]
public class Crescent : MoonPhaseCard
{
    public Crescent() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, true) { }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/Crescent.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/Crescent.png");

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike, CardTags.MoonPhase];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [CardTags.MoonPhaseHoverTip, HoverTipFactory.FromCard<FullMoon>(IsUpgraded)];
    protected override CardModel CreateOtherPhase() => CardScope!.CreateCard<FullMoon>(Owner);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        QueuePhaseChange(cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}


[RegisterCard(typeof(CardPool))]
[RegisterCharacterStarterCard(typeof(Characters.Character), 3)]
public class FullMoon : MoonPhaseCard
{
    public FullMoon() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self, true) { }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/FullMoon.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/FullMoon.png");

    public override bool GainsBlock => true;
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend, CardTags.MoonPhase];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [CardTags.MoonPhaseHoverTip, HoverTipFactory.FromCard<Crescent>(IsUpgraded)];
    protected override CardModel CreateOtherPhase() => CardScope!.CreateCard<Crescent>(Owner);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        QueuePhaseChange(cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public abstract class MoonPhaseCard(int cost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary)
    : ModCardTemplate(cost, type, rarity, target, showInCardLibrary)
{
    private bool _phaseChangePending;
    protected abstract CardModel CreateOtherPhase();

    protected void QueuePhaseChange(CardPlay play) =>
        _phaseChangePending = play.PlayIndex == play.PlayCount - 1 && !IsDupe;

    //Let the native play pipeline move the original node once, then transform in its final pile.
    public override async Task AfterCardChangedPilesLate(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card != this || !_phaseChangePending || oldPileType != PileType.Play || Pile == null) return;
        _phaseChangePending = false;
        if (!IsTransformable || MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsEnding) return;
        CardModel replacement = CreateOtherPhase();
        if (IsUpgraded) CardCmd.Upgrade(replacement, CardPreviewStyle.None);
        await CardCmd.Transform(this, replacement, CardPreviewStyle.None);
    }
}
