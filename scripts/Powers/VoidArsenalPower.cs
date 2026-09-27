using guestFromHeresy.Cards;
using guestFromHeresy.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class VoidArsenalPower : TemporaryIconPower
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext context, ICombatState combat)
    {
        if (player != Owner.Player || !Owner.IsAlive) return;
        var parts = ModelDb.Card<InverseLevel>().Pool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => c.Keywords.Contains(ArtifactPart.Value) && c.CanBeGeneratedInCombat).ToArray();
        if (parts.Length == 0) return;
        for (int i = 0; i < Amount; i++)
        {
            var part = player.RunState.Rng.CombatCardGeneration.NextItem(parts)!;
            await CardPileCmd.AddGeneratedCardToCombat(combat.CreateCard(part, player), PileType.Hand, player);
        }
    }
}
