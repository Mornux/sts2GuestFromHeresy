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
public sealed class BloomingCore() : ArtifactPartCard(1, CardType.Skill, CardRarity.Uncommon)
{
    public override int PartCode => 2;
    internal static IEnumerable<DynamicVar> EffectVars => [new PowerVar<GazedPower>(2m), new EnergyVar(2)];
    protected override IEnumerable<DynamicVar> CanonicalVars => EffectVars;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => ExecuteEffect(this, context, play);

    // The source is the part itself or the artifact executing this component.
    internal static async Task ExecuteEffect(ArtifactPartCard source, PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<GazedPower>(context, source.Owner.Creature, source.DynamicVars["GazedPower"].BaseValue, source.Owner.Creature, source);
        await PowerCmd.Apply<EnergyNextTurnPower>(context, source.Owner.Creature, source.DynamicVars.Energy.BaseValue, source.Owner.Creature, source);
    }
}
