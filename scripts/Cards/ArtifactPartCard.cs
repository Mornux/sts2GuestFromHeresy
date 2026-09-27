using guestFromHeresy.Keywords;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

// Shared part identity, presentation and forging bookkeeping. Effects live in each concrete card.
public abstract class ArtifactPartCard(int cost, CardType type, CardRarity rarity)
    : ModCardTemplate(cost, type, rarity, TargetType.Self, true)
{
    public static readonly string[] PartEntries = ["INVERSE_LEVEL", "NEGATIVE_ENERGY_SHIELD", "BLOOMING_CORE", "ARTISANS_HAMMER", "TIDAL_MARROW", "DISINTEGRATION_PROBE", "RITUAL_HILT", "ENCHANTED_MAGNET"];
    public abstract int PartCode { get; }
    public virtual IEnumerable<int> EffectCodes => [PartCode];
    public virtual bool IsPart => true;
    public override int MaxUpgradeLevel => IsPart ? 0 : 1;
    public override TargetType TargetType => EffectCodes.Any(c => c is 6 or 7) ? TargetType.AnyEnemy
        : EffectCodes.Contains(5) ? TargetType.AllEnemies : TargetType.Self;
    public override bool GainsBlock => EffectCodes.Contains(1);
    protected override bool ShouldGlowGoldInternal => EffectCodes.Contains(6) && Synaesthesia.IsActive(this);
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/TempImage.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/TempImage.png");
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            if (IsPart) yield return ArtifactPart.Value;
            if (EffectCodes.Contains(5)) yield return CardKeyword.Retain;
        }
    }
    protected override IEnumerable<IHoverTip> AdditionalHoverTips
    {
        get
        {
            if (EffectCodes.Contains(0)) yield return HoverTipFactory.FromCard<ShadowMissile>();
            if (EffectCodes.Contains(1)) { yield return HoverTipFactory.FromPower<WeakPower>(); yield return HoverTipFactory.Static(StaticHoverTip.Block); }
            if (EffectCodes.Contains(2)) { yield return HoverTipFactory.FromPower<GazedPower>(); yield return EnergyHoverTip; }
            if (EffectCodes.Contains(3)) yield return HoverTipFactory.FromPower<PlatingPower>();
            if (EffectCodes.Contains(4)) yield return CardTags.MoonPhaseHoverTip;
            if (EffectCodes.Contains(5)) yield return HoverTipFactory.FromPower<DisintegrationPower>();
            if (EffectCodes.Contains(6)) yield return ModKeywordRegistry.CreateHoverTip(Synaesthesia.Id);
            if (EffectCodes.Contains(7)) yield return HoverTipFactory.FromPower<VigorPower>();
        }
    }
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play)
    {
        if (play.Card != this || !IsPart || !Keywords.Contains(ArtifactPart.Value)
            || CombatManager.Instance.IsOverOrEnding || !Owner.Creature.IsAlive) return;
        var forge = Owner.Creature.GetPower<VoidForgingPower>();
        if (forge == null)
        {
            await PowerCmd.Apply<VoidForgingPower>(context, Owner.Creature, 1m, Owner.Creature, this);
            forge = Owner.Creature.GetPower<VoidForgingPower>();
        }
        if (forge != null) await forge.Record(this);
    }
}
