using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
namespace guestFromHeresy.Powers;
[RegisterPower]
public sealed class MoralDebtPower : TemporaryIconPower
{
    public override Task AfterSideTurnEnd(PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants) =>
        participants.Contains(Owner) && Owner.IsAlive && Owner.Player != null && !ProjectionManager.IsGazed(Owner.Player)
            ? PowerCmd.Apply<DisintegrationPower>(context, Owner, Amount, Owner, null) : Task.CompletedTask;
}
