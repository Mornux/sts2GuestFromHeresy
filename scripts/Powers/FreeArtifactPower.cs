using guestFromHeresy.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class FreeArtifactPower : TemporaryIconPower
{
    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        bool free = Amount > 0 && card.Owner?.Creature == Owner && card.Tags.Contains(CardTags.Artifact)
            && card.Pile?.Type is PileType.Hand or PileType.Play;
        modifiedCost = free ? 0m : originalCost;
        return free;
    }
    public override Task BeforeCardPlayed(CardPlay play) =>
        play.IsFirstInSeries && play.Card.Owner.Creature == Owner && play.Card.Tags.Contains(CardTags.Artifact)
            ? PowerCmd.Decrement(this) : Task.CompletedTask;
}
