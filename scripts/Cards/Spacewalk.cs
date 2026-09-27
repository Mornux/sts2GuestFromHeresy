using HarmonyLib;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class Spacewalk() : TemporaryArtPower(1, CardRarity.Uncommon)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<GazedPower>(), HoverTipFactory.Static(StaticHoverTip.Transform)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        PowerCmd.Apply<SpacewalkPower>(context, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);

    [HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Transform), [typeof(IEnumerable<CardTransformation>), typeof(Rng), typeof(CardPreviewStyle)])]
    private static class TransformationListener
    {
        private static void Postfix(ref Task<IEnumerable<CardPileAddResult>> __result)
        {
            var original = __result;
            __result = ((Func<Task<IEnumerable<CardPileAddResult>>>)(async () =>
            {
                var results = (await original).ToArray();
                foreach (var result in results)
                {
                    if (!result.success || result.cardAdded.CombatState == null || CombatManager.Instance.IsOverOrEnding) continue;
                    if (result.cardAdded.Owner.Creature.GetPower<SpacewalkPower>() is { Owner.IsAlive: true } power)
                        await PowerCmd.Apply<GazedPower>(new ThrowingPlayerChoiceContext(), power.Owner, power.Amount, power.Owner, null);
                }
                return (IEnumerable<CardPileAddResult>)results;
            }))();
        }
    }
}
