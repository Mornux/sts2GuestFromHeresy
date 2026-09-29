using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Powers;

// Independent player-side models. Native powers are used only as lookup keys and icon sources.
public static class VectorProjectionRegistry
{
    public static IReadOnlyDictionary<Type, Func<PowerModel>> Factories { get; } = new Dictionary<Type, Func<PowerModel>>
    {
        [typeof(AdaptablePower)] = () => ModelDb.Power<AdaptableVectorProjectionPower>().ToMutable(),
        [typeof(ArtifactPower)] = () => ModelDb.Power<ArtifactVectorProjectionPower>().ToMutable(),
        [typeof(AsleepPower)] = () => ModelDb.Power<AsleepVectorProjectionPower>().ToMutable(),
        [typeof(BackAttackLeftPower)] = () => ModelDb.Power<BackAttackLeftVectorProjectionPower>().ToMutable(),
        [typeof(BackAttackRightPower)] = () => ModelDb.Power<BackAttackRightVectorProjectionPower>().ToMutable(),
        [typeof(BattlewornDummyTimeLimitPower)] = () => ModelDb.Power<BattlewornDummyTimeLimitVectorProjectionPower>().ToMutable(),
        [typeof(BurrowedPower)] = () => ModelDb.Power<BurrowedVectorProjectionPower>().ToMutable(),
        [typeof(CrabRagePower)] = () => ModelDb.Power<CrabRageVectorProjectionPower>().ToMutable(),
        [typeof(CurlUpPower)] = () => ModelDb.Power<CurlUpVectorProjectionPower>().ToMutable(),
        [typeof(DexterityPower)] = () => ModelDb.Power<DexterityVectorProjectionPower>().ToMutable(),
        [typeof(EnragePower)] = () => ModelDb.Power<EnrageVectorProjectionPower>().ToMutable(),
        [typeof(EscapeArtistPower)] = () => ModelDb.Power<EscapeArtistVectorProjectionPower>().ToMutable(),
        [typeof(FlutterPower)] = () => ModelDb.Power<FlutterVectorProjectionPower>().ToMutable(),
        [typeof(GalvanicPower)] = () => ModelDb.Power<GalvanicVectorProjectionPower>().ToMutable(),
        [typeof(HardToKillPower)] = () => ModelDb.Power<HardToKillVectorProjectionPower>().ToMutable(),
        [typeof(HardenedShellPower)] = () => ModelDb.Power<HardenedShellVectorProjectionPower>().ToMutable(),
        [typeof(HatchPower)] = () => ModelDb.Power<HatchVectorProjectionPower>().ToMutable(),
        [typeof(HeistPower)] = () => ModelDb.Power<HeistVectorProjectionPower>().ToMutable(),
        [typeof(HighVoltagePower)] = () => ModelDb.Power<HighVoltageVectorProjectionPower>().ToMutable(),
        [typeof(IllusionPower)] = () => ModelDb.Power<IllusionVectorProjectionPower>().ToMutable(),
        [typeof(InfestedPower)] = () => ModelDb.Power<InfestedVectorProjectionPower>().ToMutable(),
        [typeof(IntangiblePower)] = () => ModelDb.Power<IntangibleVectorProjectionPower>().ToMutable(),
        [typeof(MinionPower)] = () => ModelDb.Power<MinionVectorProjectionPower>().ToMutable(),
        [typeof(NemesisPower)] = () => ModelDb.Power<NemesisVectorProjectionPower>().ToMutable(),
        [typeof(PainfulStabsPower)] = () => ModelDb.Power<PainfulStabsVectorProjectionPower>().ToMutable(),
        [typeof(PaperCutsPower)] = () => ModelDb.Power<PaperCutsVectorProjectionPower>().ToMutable(),
        [typeof(PersonalHivePower)] = () => ModelDb.Power<PersonalHiveVectorProjectionPower>().ToMutable(),
        [typeof(PlatingPower)] = () => ModelDb.Power<PlatingVectorProjectionPower>().ToMutable(),
        [typeof(PossessSpeedPower)] = () => ModelDb.Power<PossessSpeedVectorProjectionPower>().ToMutable(),
        [typeof(PossessStrengthPower)] = () => ModelDb.Power<PossessStrengthVectorProjectionPower>().ToMutable(),
        [typeof(RampartPower)] = () => ModelDb.Power<RampartVectorProjectionPower>().ToMutable(),
        [typeof(RavenousPower)] = () => ModelDb.Power<RavenousVectorProjectionPower>().ToMutable(),
        [typeof(ReattachPower)] = () => ModelDb.Power<ReattachVectorProjectionPower>().ToMutable(),
        [typeof(RitualPower)] = () => ModelDb.Power<RitualVectorProjectionPower>().ToMutable(),
        [typeof(SandpitPower)] = () => ModelDb.Power<SandpitVectorProjectionPower>().ToMutable(),
        [typeof(SkittishPower)] = () => ModelDb.Power<SkittishVectorProjectionPower>().ToMutable(),
        [typeof(SlipperyPower)] = () => ModelDb.Power<SlipperyVectorProjectionPower>().ToMutable(),
        [typeof(SlumberPower)] = () => ModelDb.Power<SlumberVectorProjectionPower>().ToMutable(),
        [typeof(SoarPower)] = () => ModelDb.Power<SoarVectorProjectionPower>().ToMutable(),
        [typeof(SteamEruptionPower)] = () => ModelDb.Power<SteamEruptionVectorProjectionPower>().ToMutable(),
        [typeof(StockPower)] = () => ModelDb.Power<StockVectorProjectionPower>().ToMutable(),
        [typeof(StrengthPower)] = () => ModelDb.Power<StrengthVectorProjectionPower>().ToMutable(),
        [typeof(SuckPower)] = () => ModelDb.Power<SuckVectorProjectionPower>().ToMutable(),
        [typeof(SurprisePower)] = () => ModelDb.Power<SurpriseVectorProjectionPower>().ToMutable(),
        [typeof(SwipePower)] = () => ModelDb.Power<SwipeVectorProjectionPower>().ToMutable(),
        [typeof(TerritorialPower)] = () => ModelDb.Power<TerritorialVectorProjectionPower>().ToMutable(),
        [typeof(ThieveryPower)] = () => ModelDb.Power<ThieveryVectorProjectionPower>().ToMutable(),
        [typeof(ThornsPower)] = () => ModelDb.Power<ThornsVectorProjectionPower>().ToMutable(),
        [typeof(VigorPower)] = () => ModelDb.Power<VigorVectorProjectionPower>().ToMutable(),
        [typeof(VitalSparkPower)] = () => ModelDb.Power<VitalSparkVectorProjectionPower>().ToMutable(),
        [typeof(WitheringPresencePower)] = () => ModelDb.Power<WitheringPresenceVectorProjectionPower>().ToMutable(),
    };
    public static bool CanProject(PowerModel source) => source.Type == PowerType.Buff && source.Amount > 0 && Factories.ContainsKey(source.GetType());
    public static PowerModel Create(PowerModel source) => Factories[source.GetType()]();
}

public abstract class VectorProjectionPower : ModPowerTemplate
{
    public abstract PowerModel SourceModel { get; }
    protected string Kind => SourceModel.GetType().Name;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomIconPath => SourceModel.PackedIconPath;
    public override string CustomBigIconPath => CustomIconPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Progress", 0m), new DynamicVar("Initial", 0m)];
    private bool _triggered;
    private bool _alternate;
    private decimal _hpLost;
    private int _vigorAtStart;
    private CardModel? _storedCard;
    private Creature? _markedTarget;
    public override PowerInstanceType InstanceType => Kind is "SwipePower" or "SandpitPower" ? PowerInstanceType.Instanced : PowerInstanceType.None;
    private bool IsRevival => Kind is "AdaptablePower" or "IllusionPower" or "ReattachPower" or "StockPower";
    public override bool ShouldDieLate(Creature creature) => creature != Owner || !IsRevival || Amount <= 0;
    public override async Task AfterPreventingDeath(Creature creature)
    {
        if (creature != Owner || !IsRevival) return;
        await PowerCmd.Remove(this);
        await CreatureCmd.Heal(creature, 1m);
    }
    // Same priority groups as ThievingHopper; ties use the synchronized combat RNG.
    internal static int SwipePriority(CardModel card) => card.Enchantment is Imbued ? 3 : card.Rarity switch
    {
        CardRarity.Uncommon => 0,
        CardRarity.Common or CardRarity.Rare or CardRarity.Event => 1,
        CardRarity.Basic or CardRarity.Quest => 2,
        CardRarity.Ancient => 3,
        _ => 4
    };

    protected Task Block(decimal amount) => CreatureCmd.GainBlock(Owner, amount, ValueProp.Unpowered, null);
    protected Task Strength(PlayerChoiceContext context, decimal amount) => PowerCmd.Apply<StrengthVectorProjectionPower>(context, Owner, amount, Owner, null);
    protected Task Dexterity(PlayerChoiceContext context, decimal amount) => PowerCmd.Apply<DexterityVectorProjectionPower>(context, Owner, amount, Owner, null);
    protected Task Hit(PlayerChoiceContext context, Creature target, decimal amount) => target.IsAlive
        ? CreatureCmd.Damage(context, target, amount, ValueProp.Unpowered, Owner, null) : Task.CompletedTask;
    protected async Task HitAll(PlayerChoiceContext context, decimal amount)
    {
        foreach (var enemy in Owner.CombatState!.GetOpponentsOf(Owner).Where(c => c.IsAlive).ToArray()) await Hit(context, enemy, amount);
    }
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        DynamicVars["Initial"].BaseValue = Amount;
        if (Kind == "SwipePower" && Owner.Player is { } player && player.RunState.CurrentRoom is CombatRoom room)
        {
            var cards = CardPile.GetCards(player, PileType.Draw, PileType.Discard)
                .Where(c => c.DeckVersion?.Pile?.Type == PileType.Deck).ToArray();
            if (cards.Length > 0)
            {
                var priority = cards.Min(SwipePriority);
                _storedCard = player.RunState.Rng.CombatCardSelection.NextItem(cards.Where(c => SwipePriority(c) == priority).ToArray());
                var deckCard = _storedCard!.DeckVersion!;
                await CardPileCmd.RemoveFromCombat(_storedCard);
                await CardPileCmd.RemoveFromDeck(deckCard, showPreview: false);
                player.RunState.CurrentMapPointHistoryEntry?.GetEntry(player.NetId).MarkLootStolen();
                // Register the return reward now so removing this power cannot lose the card.
                // AddCard restores run ownership only; the reward adds it back to the deck when claimed.
                player.RunState.AddCard(deckCard, player);
                var reward = new SpecialCardReward(deckCard, player);
                reward.SetCustomDescriptionEncounterSource(ModelDb.Encounter<ThievingHopperWeak>().Id);
                room.AddExtraReward(player, reward);
            }
        }
        if (Kind == "SandpitPower")
        {
            var enemies = Owner.CombatState!.GetOpponentsOf(Owner).Where(c => c.IsAlive).ToArray();
            if (enemies.Length > 0) _markedTarget = Owner.Player!.RunState.Rng.CombatTargets.NextItem(enemies);
        }
    }
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource) =>
        dealer == Owner && props.IsPoweredAttack() && Kind is "StrengthPower" or "VigorPower" ? Amount : 0m;
    public override decimal ModifyBlockAdditive(Creature target, decimal amount, ValueProp props, CardModel? cardSource, CardPlay? cardPlay) =>
        target == Owner && props.HasFlag(ValueProp.Move) && Kind == "DexterityPower" ? Amount : 0m;
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack()) return 1m;
        if (target == Owner && Kind is "FlutterPower" or "SoarPower") return 0.5m;
        if (dealer == Owner && target != null && target.Side != Owner.Side && target.Block == 0 && Kind is "BackAttackLeftPower" or "BackAttackRightPower") return 1.5m;
        return 1m;
    }
    public override decimal ModifyDamageCap(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource) =>
        target != Owner ? decimal.MaxValue : Kind == "IntangiblePower" ? 1m : Kind == "HardToKillPower" ? Amount : decimal.MaxValue;
    public override decimal ModifyHpLostBeforeOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource) =>
        target == Owner && Kind == "HardenedShellPower" ? Math.Min(amount, Math.Max(0m, Amount - _hpLost)) : amount;
    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource) =>
        target == Owner && (Kind == "IntangiblePower" || Kind == "SlipperyPower" && props.IsPoweredAttack()) ? Math.Min(amount, 1m) : amount;
    public override bool ShouldClearBlock(Creature creature) => creature != Owner || Kind != "BurrowedPower";
    public override async Task AfterBlockBroken(Creature creature)
    {
        if (creature == Owner && Kind == "BurrowedPower") await PowerCmd.Remove(this);
    }
    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        bool prevent = Kind == "ArtifactPower" && target == Owner && canonicalPower.GetTypeForAmount(amount) == PowerType.Debuff && amount != 0;
        modifiedAmount = prevent ? 0m : amount;
        return prevent;
    }
    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        if (Kind == "ArtifactPower") await PowerCmd.Decrement(this);
    }
    public override Task BeforeSideTurnStart(PlayerChoiceContext context, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combat)
    {
        _hpLost = 0;
        if (participants.Contains(Owner)) { _triggered = false; }
        return Task.CompletedTask;
    }
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combat)
    {
        if (!participants.Contains(Owner) || !Owner.IsAlive) return;
        if (Kind == "RampartPower") await Block(Amount);
        if (Kind == "PlatingPower") await PowerCmd.Decrement(this);
    }
    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner) && Owner.IsAlive && Kind is "PlatingPower" or "AsleepPower" or "SlumberPower") await Block(Amount);
    }
    public override async Task AfterSideTurnEnd(PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!Owner.IsAlive) return;
        if (Kind == "SkittishPower" && side != Owner.Side) _triggered = false;
        if (Kind == "IntangiblePower" && side != Owner.Side) { await PowerCmd.Decrement(this); return; }
        if (!participants.Contains(Owner)) return;
        switch (Kind)
        {
            case "SandpitPower":
                if (Amount > 1) await PowerCmd.Decrement(this);
                else { await PowerCmd.Remove(this); if (_markedTarget?.IsAlive == true) await CreatureCmd.Kill(_markedTarget); }
                break;
            case "RitualPower": case "HighVoltagePower": case "TerritorialPower": await Strength(context, Amount); break;
            case "AsleepPower": case "SlumberPower": await PowerCmd.Decrement(this); break;
            case "NemesisPower":
                _alternate = !_alternate;
                if (_alternate) await PowerCmd.Apply<IntangibleVectorProjectionPower>(context, Owner, 1m, Owner, null);
                break;
        }
    }
    public override Task BeforeAttack(AttackCommand command)
    {
        if (Kind == "VigorPower" && command.Attacker == Owner && command.DamageProps.IsPoweredAttack()) _vigorAtStart = Amount;
        return Task.CompletedTask;
    }
    public override async Task AfterAttack(PlayerChoiceContext context, AttackCommand command)
    {
        if (!Owner.IsAlive || !command.DamageProps.IsPoweredAttack()) return;
        var received = command.Results.SelectMany(r => r).Where(r => r.Receiver == Owner).ToArray();
        if (Kind == "SkittishPower" && !_triggered && received.Any(r => r.UnblockedDamage > 0)) { _triggered = true; await Block(Amount); }
        if (Kind == "CurlUpPower" && received.Length > 0) { await Block(Amount); await PowerCmd.Remove(this); }
        if (command.Attacker != Owner) return;
        if (Kind == "VigorPower" && _vigorAtStart > 0)
        {
            int used = _vigorAtStart; _vigorAtStart = 0;
            await PowerCmd.ModifyAmount(context, this, -used, Owner, null);
        }
        if (Kind == "SuckPower")
        {
            int hits = command.Results.Count(batch => batch.Any(r => r.Receiver.Side != Owner.Side && r.UnblockedDamage > 0));
            if (hits > 0) await Strength(context, Amount * hits);
        }
    }
    public override async Task BeforeDamageReceived(PlayerChoiceContext context, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Kind == "ThornsPower" && target == Owner && dealer != null && dealer.Side != Owner.Side && props.IsPoweredAttack()) await Hit(context, dealer, Amount);
    }
    public override async Task AfterDamageReceived(PlayerChoiceContext context, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || !Owner.IsAlive) return;
        if (Kind == "HardenedShellPower") _hpLost += Math.Max(0, result.UnblockedDamage);
        if (Kind == "PersonalHivePower" && dealer != null && dealer.Side != Owner.Side && props.IsPoweredAttack())
            await PowerCmd.Apply<WeakPower>(context, dealer, Amount, Owner, null);
        if (result.UnblockedDamage <= 0) return;
        if (Kind == "SteamEruptionPower") { int damage = Amount; await PowerCmd.Remove(this); await HitAll(context, damage); }
        if (Kind == "AsleepPower") await PowerCmd.Remove(this);
        if (Kind == "SlumberPower" || (props.IsPoweredAttack() && Kind is "FlutterPower" or "SlipperyPower")) await PowerCmd.Decrement(this);
    }
    public override async Task AfterDamageGiven(PlayerChoiceContext context, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer != Owner || target.Side == Owner.Side || result.UnblockedDamage <= 0 || !props.IsPoweredAttack()) return;
        if (Kind is "HeistPower" or "ThieveryPower") await PlayerCmd.GainGold(Amount, Owner.Player!);
        if (!target.IsAlive) return;
        switch (Kind)
        {
            case "PainfulStabsPower": await PowerCmd.Apply<DisintegrationPower>(context, target, Amount, Owner, cardSource); break;
            case "PaperCutsPower": await CreatureCmd.LoseMaxHp(context, target, Amount, isFromCard: false); break;
            case "VitalSparkPower": await PowerCmd.Apply<DisintegrationPower>(context, target, Amount, Owner, cardSource); break;
            case "PossessStrengthPower":
                if (_triggered) break; _triggered = true;
                await PowerCmd.Apply<StrengthPower>(context, target, -Amount, Owner, null); await Strength(context, Amount); break;
            case "PossessSpeedPower":
                if (_triggered) break; _triggered = true;
                await PowerCmd.Apply<DexterityPower>(context, target, -Amount, Owner, null); await Dexterity(context, Amount); break;
        }
    }
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play)
    {
        if (play.Card.Owner.Creature != Owner || !Owner.IsAlive) return;
        if (Kind == "EnragePower" && play.Card.Type == CardType.Skill) await Strength(context, Amount);
        if (Kind == "GalvanicPower" && play.Card.Type == CardType.Skill)
        {
            var enemies = Owner.CombatState!.GetOpponentsOf(Owner).Where(c => c.IsAlive).ToArray();
            if (enemies.Length > 0) await Hit(context, Owner.Player!.RunState.Rng.CombatTargets.NextItem(enemies)!, Amount);
        }
        if (Kind == "WitheringPresencePower")
        {
            var progress = DynamicVars["Progress"]; progress.BaseValue++;
            if (progress.IntValue >= 6)
            {
                progress.BaseValue -= 6;
                await PowerCmd.Apply<DisintegrationPower>(context, Owner.CombatState!.GetOpponentsOf(Owner).Where(c => c.IsAlive), Amount, Owner, null);
            }
        }
    }
    public override async Task AfterDeath(PlayerChoiceContext context, Creature target, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || target == Owner || !Owner.IsAlive) return;
        if (Kind == "RavenousPower" && target.Side != Owner.Side) await Strength(context, Amount);
        if (Kind == "CrabRagePower") { await Strength(context, 6); await Block(99); await PowerCmd.Remove(this); }
    }
}

[RegisterPower]
public sealed class AdaptableVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<AdaptablePower>();
}

[RegisterPower]
public sealed class ArtifactVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<ArtifactPower>();
}

[RegisterPower]
public sealed class AsleepVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<AsleepPower>();
}

[RegisterPower]
public sealed class BackAttackLeftVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<BackAttackLeftPower>();
}

[RegisterPower]
public sealed class BackAttackRightVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<BackAttackRightPower>();
}

[RegisterPower]
public sealed class BattlewornDummyTimeLimitVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<BattlewornDummyTimeLimitPower>();
}

[RegisterPower]
public sealed class BurrowedVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<BurrowedPower>();
}

[RegisterPower]
public sealed class CrabRageVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<CrabRagePower>();
}

[RegisterPower]
public sealed class CurlUpVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<CurlUpPower>();
}

[RegisterPower]
public sealed class DexterityVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<DexterityPower>();
}

[RegisterPower]
public sealed class EnrageVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<EnragePower>();
}

[RegisterPower]
public sealed class EscapeArtistVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<EscapeArtistPower>();
}

[RegisterPower]
public sealed class FlutterVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<FlutterPower>();
}

[RegisterPower]
public sealed class GalvanicVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<GalvanicPower>();
}

[RegisterPower]
public sealed class HardToKillVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<HardToKillPower>();
}

[RegisterPower]
public sealed class HardenedShellVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<HardenedShellPower>();
}

[RegisterPower]
public sealed class HatchVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<HatchPower>();
}

[RegisterPower]
public sealed class HeistVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<HeistPower>();
}

[RegisterPower]
public sealed class HighVoltageVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<HighVoltagePower>();
}

[RegisterPower]
public sealed class IllusionVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<IllusionPower>();
}

[RegisterPower]
public sealed class InfestedVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<InfestedPower>();
}

[RegisterPower]
public sealed class IntangibleVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<IntangiblePower>();
}

[RegisterPower]
public sealed class MinionVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<MinionPower>();
}

[RegisterPower]
public sealed class NemesisVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<NemesisPower>();
}

[RegisterPower]
public sealed class PainfulStabsVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<PainfulStabsPower>();
}

[RegisterPower]
public sealed class PaperCutsVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<PaperCutsPower>();
}

[RegisterPower]
public sealed class PersonalHiveVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<PersonalHivePower>();
}

[RegisterPower]
public sealed class PlatingVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<PlatingPower>();
}

[RegisterPower]
public sealed class PossessSpeedVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<PossessSpeedPower>();
}

[RegisterPower]
public sealed class PossessStrengthVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<PossessStrengthPower>();
}

[RegisterPower]
public sealed class RampartVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<RampartPower>();
}

[RegisterPower]
public sealed class RavenousVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<RavenousPower>();
}

[RegisterPower]
public sealed class ReattachVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<ReattachPower>();
}

[RegisterPower]
public sealed class RitualVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<RitualPower>();
}

[RegisterPower]
public sealed class SandpitVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<SandpitPower>();
}

[RegisterPower]
public sealed class SkittishVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<SkittishPower>();
}

[RegisterPower]
public sealed class SlipperyVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<SlipperyPower>();
}

[RegisterPower]
public sealed class SlumberVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<SlumberPower>();
}

[RegisterPower]
public sealed class SoarVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<SoarPower>();
}

[RegisterPower]
public sealed class SteamEruptionVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<SteamEruptionPower>();
}

[RegisterPower]
public sealed class StockVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<StockPower>();
}

[RegisterPower]
public sealed class StrengthVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<StrengthPower>();
}

[RegisterPower]
public sealed class SuckVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<SuckPower>();
}

[RegisterPower]
public sealed class SurpriseVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<SurprisePower>();
}

[RegisterPower]
public sealed class SwipeVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<SwipePower>();
}

[RegisterPower]
public sealed class TerritorialVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<TerritorialPower>();
}

[RegisterPower]
public sealed class ThieveryVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<ThieveryPower>();
}

[RegisterPower]
public sealed class ThornsVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<ThornsPower>();
}

[RegisterPower]
public sealed class VigorVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<VigorPower>();
}

[RegisterPower]
public sealed class VitalSparkVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<VitalSparkPower>();
}

[RegisterPower]
public sealed class WitheringPresenceVectorProjectionPower : VectorProjectionPower
{
    public override PowerModel SourceModel => ModelDb.Power<WitheringPresencePower>();
}

