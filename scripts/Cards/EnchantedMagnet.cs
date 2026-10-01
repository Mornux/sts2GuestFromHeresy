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
public sealed class EnchantedMagnet() : ArtifactPartCard(1, CardType.Attack, CardRarity.Common)
{
    public override int PartCode => 7;
    internal static IEnumerable<DynamicVar> EffectVars => [new DamageVar("MagnetDamage", 13m, ValueProp.Move), new PowerVar<VigorPower>(6m)];
    protected override IEnumerable<DynamicVar> CanonicalVars => EffectVars;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => ExecuteEffect(this, context, play);

    // The source is the part itself or the artifact executing this component.
    internal static async Task ExecuteEffect(ArtifactPartCard source, PlayerChoiceContext context, CardPlay play)
    {
        var target = play.Target;
        if (target?.IsAlive != true)
            target = source.Owner.RunState.Rng.CombatTargets.NextItem(source.CombatState!.HittableEnemies.ToArray());
        if (target == null) return;
        await DamageCmd.Attack(source.DynamicVars["MagnetDamage"].BaseValue).FromCard(source, play).Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(context);
        if (!CombatManager.Instance.IsOverOrEnding && source.Owner.Creature.IsAlive)
            await PowerCmd.Apply<VigorPower>(context, source.Owner.Creature, source.DynamicVars["VigorPower"].BaseValue, source.Owner.Creature, source);
    }
}
