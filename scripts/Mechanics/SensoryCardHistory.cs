using guestFromHeresy.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
namespace guestFromHeresy.Mechanics;
public static class SensoryCardHistory
{
    public static bool IsZeroCost(CardPlay play) => !play.Card.EnergyCost.CostsX && play.Resources.EnergyValue == 0;
    public static int ZeroCostPlays(CardModel card) => card.CombatState == null ? 0 : CombatManager.Instance.History.CardPlaysFinished.Count(e => e.CardPlay.Card.Owner == card.Owner && IsZeroCost(e.CardPlay));
    public static bool PlayedMissileThisTurn(CardModel card) => card.CombatState != null && CombatManager.Instance.History.CardPlaysFinished.Any(e => e.CardPlay.Card.Owner == card.Owner && e.CardPlay.Card is ShadowMissile && e.HappenedThisTurn(card.CombatState));
}
