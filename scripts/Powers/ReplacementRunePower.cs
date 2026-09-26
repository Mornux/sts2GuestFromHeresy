using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using guestFromHeresy.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class ReplacementRunePower : TemporaryIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    private MonsterModel? _triggeringMonster;
    private bool _triggeredOutsideMove;
    internal CardModel ReplaceGeneratedCard(CardModel card)
    {
        if (card.Owner != Owner.Player || card.Type != CardType.Status || card.Pile != null) return card;
        var monster = Owner.CombatState!.Enemies.Select(enemy => enemy.Monster).FirstOrDefault(model => model?.IsPerformingMove == true);
        if (_triggeringMonster != null && monster != _triggeringMonster) return card;
        _triggeringMonster = monster;
        _triggeredOutsideMove |= monster == null;
        var missile = Owner.CombatState.CreateCard<ShadowMissile>(Owner.Player!);
        card.RemoveFromState();
        return missile;
    }
    internal Task FinishMove(MonsterModel monster) =>
        _triggeringMonster == monster ? PowerCmd.Remove(this) : Task.CompletedTask;
    internal Task FinishGeneration() => _triggeredOutsideMove ? PowerCmd.Remove(this) : Task.CompletedTask;
}

// Enemy powers can also generate a batch outside a monster's move.
[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.AddGeneratedCardsToCombat))]
internal static class ReplacementRuneGenerationPatch
{
    // Replace before pile entry: no status draw/entry effects run, and previews show the missile.
    private static void Prefix(ref IEnumerable<CardModel> cards, Player? creator)
    {
        if (creator != null || MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsOverOrEnding) return;
        cards = cards.Select(card => card.Owner.Creature.GetPower<ReplacementRunePower>() is { } rune
            ? rune.ReplaceGeneratedCard(card) : card).ToArray();
    }
    private static void Postfix(Player? creator, ref Task<IReadOnlyList<CardPileAddResult>> __result)
    {
        if (creator == null) __result = Finish(__result);
    }
    private static async Task<IReadOnlyList<CardPileAddResult>> Finish(Task<IReadOnlyList<CardPileAddResult>> generation)
    {
        var results = await generation;
        foreach (var player in results.Where(result => result.cardAdded != null)
                     .Select(result => result.cardAdded.Owner).Distinct())
            if (player.Creature.GetPower<ReplacementRunePower>() is { } rune) await rune.FinishGeneration();
        return results;
    }
}

// A monster can generate several statuses via separate commands in one move.
[HarmonyPatch(typeof(MonsterModel), nameof(MonsterModel.PerformMove))]
internal static class ReplacementRuneMovePatch
{
    private static void Postfix(MonsterModel __instance, ref Task __result) => __result = Finish(__result, __instance);
    private static async Task Finish(Task move, MonsterModel monster)
    {
        await move;
        if (monster.Creature.CombatState is not { } combat) return;
        foreach (var player in combat.Players)
            if (player.Creature.GetPower<ReplacementRunePower>() is { } rune) await rune.FinishMove(monster);
    }
}
