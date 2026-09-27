using HarmonyLib;
using MegaCrit.Sts2.Core.Random;
using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using guestFromHeresy.Projection;
using guestFromHeresy.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class GravityIllusion() : TemporaryArtPower(1, CardRarity.Uncommon)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("BlockAmount", 3m)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block), HoverTipFactory.Static(StaticHoverTip.Transform)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        PowerCmd.Apply<GravityIllusionPower>(context, Owner.Creature, DynamicVars["BlockAmount"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["BlockAmount"].UpgradeValueBy(1m);

    // The game exposes transformation through this command, so the card owns its listener here.
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
                    if (result.cardAdded.Owner.Creature.GetPower<GravityIllusionPower>() is { Owner.IsAlive: true } power)
                        await CreatureCmd.GainBlock(power.Owner, power.Amount, ValueProp.Unpowered, null);
                }
                return (IEnumerable<CardPileAddResult>)results;
            }))();
        }
    }
}
