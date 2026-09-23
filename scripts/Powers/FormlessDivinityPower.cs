using guestFromHeresy.Cards;
using guestFromHeresy.Capabilities;
using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models.Capabilities;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class FormlessDivinityPower : TemporaryIconPower
{
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public async Task Choose(PlayerChoiceContext context)
    {
        var player = Owner.Player!;
        var pool = ModelDb.AllCharacters.Where(c => c.Id != player.Character.Id)
            .SelectMany(c => c.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)).Distinct();
        var cards = CardFactory.GetDistinctForCombat(player, pool, Amount, player.RunState.Rng.CombatCardGeneration).ToArray();
        foreach (var card in cards) card.GetOrCreateCapability<SynaestheticFreeCost>();
        if (cards.Length == 0) return;
        var chosen = await CardSelectCmd.FromChooseACardScreen(context, cards, player);
        if (chosen != null) await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, player);
    }
    public override Task AfterPlayerTurnStart(PlayerChoiceContext context, Player player) => player == Owner.Player ? Choose(context) : Task.CompletedTask;
}
