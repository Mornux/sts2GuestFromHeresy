using HarmonyLib;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;
namespace guestFromHeresy.Mechanics;
[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Transform), [typeof(IEnumerable<CardTransformation>), typeof(Rng), typeof(CardPreviewStyle)])]
public static class CardTransformationPatch
{
    private static void Postfix(ref Task<IEnumerable<CardPileAddResult>> __result) => __result = Resolve(__result);
    private static async Task<IEnumerable<CardPileAddResult>> Resolve(Task<IEnumerable<CardPileAddResult>> original)
    {
        var results = (await original).ToArray();
        foreach (var result in results)
        {
            if (!result.success || result.cardAdded.CombatState == null || CombatManager.Instance.IsOverOrEnding) continue;
            if (result.cardAdded.Owner.Creature.GetPower<GravityIllusionPower>() is { } power) await power.OnTransformed();
        }
        return results;
    }
}
