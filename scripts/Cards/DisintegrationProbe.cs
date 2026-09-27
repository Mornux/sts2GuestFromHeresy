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
public sealed class DisintegrationProbe() : ArtifactPartCard(1, CardType.Attack, CardRarity.Common)
{
    public override int PartCode => 5;
    internal static IEnumerable<DynamicVar> EffectVars => [new DamageVar("ProbeDamage", 2m, ValueProp.Move), new PowerVar<DisintegrationPower>(6m)];
    protected override IEnumerable<DynamicVar> CanonicalVars => EffectVars;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => ExecuteEffect(this, context, play);

    // The source is the part itself or the artifact executing this component.
    internal static async Task ExecuteEffect(ArtifactPartCard source, PlayerChoiceContext context, CardPlay play)
    {
        await DamageCmd.Attack(source.DynamicVars["ProbeDamage"].BaseValue).FromCard(source)
            .TargetingAllOpponents(source.CombatState!).WithHitFx("vfx/vfx_attack_slash").Execute(context);
        if (!CombatManager.Instance.IsOverOrEnding)
            await PowerCmd.Apply<DisintegrationPower>(context, source.CombatState!.HittableEnemies.ToArray(), source.DynamicVars["DisintegrationPower"].BaseValue, source.Owner.Creature, source);
    }
}
