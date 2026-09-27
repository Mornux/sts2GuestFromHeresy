using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using guestFromHeresy.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class NegativeEnergyShield() : ArtifactPartCard(1, CardType.Skill, CardRarity.Common)
{
    public override int PartCode => 1;
    internal static IEnumerable<DynamicVar> EffectVars => [new PowerVar<WeakPower>(2m), new BlockVar(2m, ValueProp.Move)];
    protected override IEnumerable<DynamicVar> CanonicalVars => EffectVars;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => ExecuteEffect(this, context, play);

    // The source is the part itself or the artifact executing this component.
    internal static async Task ExecuteEffect(ArtifactPartCard source, PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<WeakPower>(context, source.CombatState!.HittableEnemies.ToArray(), source.DynamicVars["WeakPower"].BaseValue, source.Owner.Creature, source);
        int weak = source.CombatState.HittableEnemies.Sum(e => Math.Max(0, e.GetPowerAmount<WeakPower>()));
        await CreatureCmd.GainBlock(source.Owner.Creature, weak * source.DynamicVars.Block.BaseValue, ValueProp.Move, play);
    }
}
