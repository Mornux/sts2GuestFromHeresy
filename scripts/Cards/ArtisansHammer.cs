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
public sealed class ArtisansHammer() : ArtifactPartCard(1, CardType.Skill, CardRarity.Uncommon)
{
    public override int PartCode => 3;
    internal static IEnumerable<DynamicVar> EffectVars => [new PowerVar<PlatingPower>(2m)];
    protected override IEnumerable<DynamicVar> CanonicalVars => EffectVars;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => ExecuteEffect(this, context, play);

    // The source is the part itself or the artifact executing this component.
    internal static async Task ExecuteEffect(ArtifactPartCard source, PlayerChoiceContext context, CardPlay play)
    {
        var selected = await CardSelectCmd.FromHand(context, source.Owner,
            new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 2), c => c.IsUpgradable, source);
        foreach (var card in selected) CardCmd.Upgrade(card);
        await PowerCmd.Apply<PlatingPower>(context, source.Owner.Creature, source.DynamicVars["PlatingPower"].BaseValue, source.Owner.Creature, source);
    }
}
