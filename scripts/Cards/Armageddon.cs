using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.ValueProps;

namespace guestFromHeresy.Cards;

[RegisterCard(typeof(CardPool))]
public sealed class Armageddon() : TemporaryArtSkill(0, CardRarity.Rare)
{
    protected override string PortraitName => nameof(Armageddon);

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawPile = PileType.Draw.GetPile(Owner);
        var cards = drawPile.Cards.ToArray();
        foreach (var card in cards)
        {
            if (Owner.Creature.IsDead) break;
            if (card.Pile == drawPile) await CardCmd.AutoPlay(choiceContext, card, null);
        }
        if (!Owner.Creature.IsDead)
            await CreatureCmd.Damage(choiceContext, Owner.Creature, 666m, ValueProp.Unpowered, Owner.Creature, this);
    }

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Retain);
}
