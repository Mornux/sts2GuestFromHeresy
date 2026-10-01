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

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawPile = PileType.Draw.GetPile(Owner);
        var cards = drawPile.Cards.Take(IsUpgraded ? int.MaxValue : 6).ToArray();
        foreach (var card in cards)
        {
            if (Owner.Creature.IsDead) break;
            if (card.Pile == drawPile) await CardCmd.AutoPlay(choiceContext, card, null);
        }
        if (!Owner.Creature.IsDead)
            await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.DisintegrationPower>(choiceContext, Owner.Creature, 666m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
