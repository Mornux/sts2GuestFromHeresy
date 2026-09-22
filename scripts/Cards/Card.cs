using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public class Strike : ModCardTemplate
{
    public Strike() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, true) { }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/Crescent.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/Crescent.png");

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike, CardTags.MoonPhase];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await MoonPhaseTransform.Into<Defend>(this, cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

[RegisterCard(typeof(CardPool))]
public class Defend : ModCardTemplate
{
    public Defend() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self, true) { }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/FullMoon.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/FullMoon.png");

    public override bool GainsBlock => true;
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend, CardTags.MoonPhase];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await MoonPhaseTransform.Into<Strike>(this, cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

internal static class MoonPhaseTransform
{
    public static async Task Into<T>(CardModel original, CardPlay cardPlay) where T : CardModel
    {
        CardModel replacement = original.CardScope!.CreateCard<T>(original.Owner);
        if (original.IsUpgraded)
        {
            CardCmd.Upgrade(replacement, CardPreviewStyle.None);
        }

        var result = await CardCmd.Transform(original, replacement, CardPreviewStyle.None);
        if (result is { } transformed)
        {
            await CardPileCmd.Add(transformed.cardAdded, cardPlay.ResultPile);
        }
    }
}
