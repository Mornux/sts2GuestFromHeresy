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
public sealed class InverseLevel() : ArtifactPartCard(1, CardType.Skill, CardRarity.Common)
{
    public override int PartCode => 0;
    internal static IEnumerable<DynamicVar> EffectVars => [new CardsVar(3)];
    protected override IEnumerable<DynamicVar> CanonicalVars => EffectVars;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => ExecuteEffect(this, context, play);

    // The source is the part itself or the artifact executing this component.
    internal static async Task ExecuteEffect(ArtifactPartCard source, PlayerChoiceContext context, CardPlay play)
    {
        await CardPileCmd.Draw(context, source.DynamicVars.Cards.BaseValue, source.Owner);
        await CardPileCmd.AddGeneratedCardToCombat(source.CardScope!.CreateCard<ShadowMissile>(source.Owner), PileType.Hand, source.Owner);
    }
}
