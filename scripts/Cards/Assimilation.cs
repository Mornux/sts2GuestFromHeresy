using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class Assimilation() : ModCardTemplate(-1, CardType.Status, CardRarity.Status, TargetType.None, true)
{
    private ModelId? _copiedPower;
    private int _copiedAmount;
    public override int MaxUpgradeLevel => 0;
    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/TempImage.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/TempImage.png");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Unplayable];
    public static bool CanCopy(PowerModel power) => power.Type == PowerType.Buff && power.Amount > 0
        // These powers control monster phases, summons, theft, or monster-only targets.
        && power is not (AdaptablePower or AsleepPower or BurrowedPower or EscapeArtistPower
            or FlutterPower or IllusionPower or InfestedPower or RampartPower or RavenousPower
            or ReattachPower or SandpitPower or SlumberPower or SteamEruptionPower or StockPower
            or SurprisePower or HeistPower or SwipePower or ThieveryPower or WitheringPresencePower);

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card != this || Pile?.Type != PileType.Hand || oldPileType == PileType.Hand) return;
        _copiedPower = null;
        _copiedAmount = 0;
        var buffs = CombatState!.Enemies.Where(c => c.IsAlive).SelectMany(c => c.Powers)
            .Where(CanCopy).ToArray();
        if (buffs.Length == 0) return;
        var selected = Owner.RunState.Rng.CombatCardSelection.NextItem(buffs)!;
        var copy = ModelDb.GetById<PowerModel>(selected.Id).ToMutable();
        int before = Owner.Creature.GetPowerInstances(selected.Id).Sum(p => p.Amount);
        await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), copy, Owner.Creature, selected.Amount, Owner.Creature, this);
        _copiedPower = selected.Id;
        _copiedAmount = Math.Max(0, Owner.Creature.GetPowerInstances(selected.Id).Sum(p => p.Amount) - before);
    }
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("HasCopy", IsMutable && Pile?.Type == PileType.Hand && _copiedPower != null && _copiedAmount > 0);
        description.Add("CopiedAmount", _copiedAmount);
        description.Add("CopiedPower", _copiedPower == null ? "" : ModelDb.GetById<PowerModel>(_copiedPower).Title.GetFormattedText());
    }
}
