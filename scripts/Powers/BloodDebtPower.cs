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
public sealed class BloodDebtPower : TemporaryIconPower
{
    private decimal _lastHp;
    public override Task AfterApplied(Creature? applier, CardModel? source)
    {
        _lastHp = Owner.CurrentHp;
        return Task.CompletedTask;
    }
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != Owner) return;
        // Projection damage also fires this hook; only real HP changes count.
        decimal lost = Math.Max(0m, _lastHp - Owner.CurrentHp);
        _lastHp = Owner.CurrentHp;
        if (lost > 0 && Owner.IsAlive && Owner.CombatState is { } combat)
            await PowerCmd.Apply<DisintegrationPower>(new ThrowingPlayerChoiceContext(), combat.HittableEnemies, lost * Amount, Owner, null);
    }
}
