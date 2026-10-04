using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class LivingAbyssPower : TemporaryIconPower
{
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play)
    {
        if (play.Card.Type != CardType.Attack || play.Card.Owner.Creature != Owner || !Owner.IsAlive || CombatManager.Instance.IsOverOrEnding) return;
        var enemies = Owner.CombatState!.HittableEnemies.Where(c => c.IsAlive).ToArray();
        if (enemies.Length == 0) return;
        var target = Owner.Player!.RunState.Rng.CombatTargets.NextItem(enemies)!;
        await PowerCmd.Apply<DisintegrationPower>(context, target, Amount, Owner, play.Card);
    }
    public override Task AfterSideTurnEnd(PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants) =>
        participants.Contains(Owner) ? PowerCmd.Remove(this) : Task.CompletedTask;
}
