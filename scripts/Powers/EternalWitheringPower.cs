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
    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature != Owner || !Owner.IsAlive) return;
        for (int i = 0; i < amount && Owner.IsAlive; i++)
        {
            var context = new ThrowingPlayerChoiceContext();
            await PowerCmd.Apply<DisintegrationPower>(context, Owner.CombatState!.HittableEnemies.ToArray(), Amount, Owner, card);
            foreach (var enemy in Owner.CombatState.HittableEnemies.ToArray())
                if (enemy.IsAlive && enemy.GetPower<DisintegrationPower>() is { } power)
                    await DisintegrationTriggers.Trigger(context, power);
        }
    }
}
