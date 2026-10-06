using guestFromHeresy.Pools;
using guestFromHeresy.Powers;
using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class BurntOffering() : TemporaryArtSkill(2, CardRarity.Rare)
{
    protected override string PortraitName => nameof(BurntOffering);

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("MaxHp", 2m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cards = PileType.Hand.GetPile(Owner).Cards.ToArray();
        foreach (var card in cards)
        {
            await CardCmd.Exhaust(choiceContext, card);
            ProjectionManager.ProjectionGainMaxHp(Owner, DynamicVars["MaxHp"].IntValue);
            await PowerCmd.Apply<GazedPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        AddKeyword(CardKeyword.Retain);
        DynamicVars["MaxHp"].UpgradeValueBy(1m);
    }
}
