using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class Illusion : ModCardTemplate
{
    public Illusion() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/Illusion.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/Illusion.png");
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selected = PileType.Draw.GetPile(Owner).Cards.ToList()
            .StableShuffle(Owner.RunState.Rng.CombatCardSelection)
            .Take(DynamicVars.Cards.IntValue).ToArray();
        foreach (var original in selected)
        {
            var copy = original.CreateClone();
            copy.AddKeyword(CardKeyword.Ethereal);
            await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}
