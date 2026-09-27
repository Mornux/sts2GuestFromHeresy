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
public sealed class RitualHilt() : ArtifactPartCard(1, CardType.Attack, CardRarity.Uncommon)
{
    public override int PartCode => 6;
    internal static IEnumerable<DynamicVar> EffectVars => [new DamageVar("HiltDamage", 6m, ValueProp.Move)];
    protected override IEnumerable<DynamicVar> CanonicalVars => EffectVars;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => ExecuteEffect(this, context, play);

    // The source is the part itself or the artifact executing this component.
    internal static async Task ExecuteEffect(ArtifactPartCard source, PlayerChoiceContext context, CardPlay play)
    {
        var target = play.Target;
        if (target?.IsAlive != true)
            target = source.Owner.RunState.Rng.CombatTargets.NextItem(source.CombatState!.HittableEnemies.ToArray());
        if (target == null) return;
        decimal damage = source.DynamicVars["HiltDamage"].BaseValue * (Synaesthesia.IsActive(source) ? 2 : 1);
        await DamageCmd.Attack(damage).FromCard(source).Targeting(target).WithHitCount(2)
            .WithHitFx("vfx/vfx_attack_slash").Execute(context);
    }
}
