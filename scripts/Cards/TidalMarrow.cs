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
public sealed class TidalMarrow() : ArtifactPartCard(1, CardType.Skill, CardRarity.Common)
{
    public override int PartCode => 4;
    internal static IEnumerable<DynamicVar> EffectVars => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => EffectVars;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => ExecuteEffect(this, context, play);

    // The source is the part itself or the artifact executing this component.
    internal static async Task ExecuteEffect(ArtifactPartCard source, PlayerChoiceContext context, CardPlay play)
    {
        // Snapshot the two cards: transforming the first cannot make it qualify a second time.
        var moons = PileType.Draw.GetPile(source.Owner).Cards.Where(c => c.Tags.Contains(CardTags.MoonPhase)).Take(2).ToArray();
        foreach (var moon in moons)
            if (moon.Pile?.Type == PileType.Draw && !CombatManager.Instance.IsOverOrEnding)
                await CardCmd.AutoPlay(context, moon, null);
    }
}
