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
public sealed class EternalWitheringPower : TemporaryIconPower
{
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play) => play.Card.Owner.Creature == Owner && Owner.IsAlive
        ? PowerCmd.Apply<DisintegrationPower>(context, Owner.CombatState!.HittableEnemies, Amount, Owner, play.Card) : Task.CompletedTask;
}
