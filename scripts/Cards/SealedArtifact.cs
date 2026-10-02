using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using guestFromHeresy.Keywords;
using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class SealedArtifact() : ArtifactPartCard(2, CardType.Attack, CardRarity.Ancient)
{
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/SealedArtifact.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/SealedArtifact.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        InverseLevel.EffectVars.Concat(NegativeEnergyShield.EffectVars).Concat(BloomingCore.EffectVars)
            .Concat(ArtisansHammer.EffectVars).Concat(TidalMarrow.EffectVars).Concat(DisintegrationProbe.EffectVars)
            .Concat(RitualHilt.EffectVars).Concat(EnchantedMagnet.EffectVars);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        foreach (int code in EffectCodes.ToArray())
        {
            if (CombatManager.Instance.IsOverOrEnding || Owner.Creature.IsDead) break;
            switch (code)
            {
                case 0: await InverseLevel.ExecuteEffect(this, context, play); break;
                case 1: await NegativeEnergyShield.ExecuteEffect(this, context, play); break;
                case 2: await BloomingCore.ExecuteEffect(this, context, play); break;
                case 3: await ArtisansHammer.ExecuteEffect(this, context, play); break;
                case 4: await TidalMarrow.ExecuteEffect(this, context, play); break;
                case 5: await DisintegrationProbe.ExecuteEffect(this, context, play); break;
                case 6: await RitualHilt.ExecuteEffect(this, context, play); break;
                case 7: await EnchantedMagnet.ExecuteEffect(this, context, play); break;
            }
        }
    }

    // The leading 1 distinguishes an empty artifact from the valid 000 recipe.
    // Value-type saved state follows native card clones, transformations and saves.
    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int Blueprint { get; set; }
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Append(CardTags.ArtifactHoverTip);
    public override int PartCode => -1;
    public override bool IsPart => false;
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    protected override HashSet<CardTag> CanonicalTags => [CardTags.Artifact];
    public override IEnumerable<int> EffectCodes => Blueprint < 0x1000 ? [] :
        new[] { (Blueprint >> 8) & 15, (Blueprint >> 4) & 15, Blueprint & 15 };
    public override string Title => TitleLocString.GetFormattedText()
        + (Blueprint >= 0x1000 ? (Blueprint & 0xFFF).ToString("X3") : "") + (IsUpgraded ? "+" : "");

    public void SetBlueprint(int recipe)
    {
        AssertMutable();
        Blueprint = 0x1000 | (recipe & 0xFFF);
        if (EffectCodes.Contains(5)) AddKeyword(CardKeyword.Retain);
    }
    protected override void AfterDeserialized()
    {
        base.AfterDeserialized();
        if (Blueprint >= 0x1000) SetBlueprint(Blueprint);
    }
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        DynamicVars.Energy.ColorPrefix = EnergyIconHelper.GetPrefix(this);
        description.Add("Effects", string.Join(" ", EffectCodes.Select(code =>
        {
            var effect = new LocString("cards", $"GUEST_FROM_HERESY_ARTIFACT_EFFECT_{code}.description");
            DynamicVars.AddTo(effect);
            effect.Add("energyPrefix", Pool.EnergyColorName);
            return effect.GetFormattedText();
        })));
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
