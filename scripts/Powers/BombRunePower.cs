using guestFromHeresy.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands.Builders;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class BombRunePower : TemporaryIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override async Task AfterAttack(PlayerChoiceContext context, AttackCommand command)
    {
        var dealer = command.Attacker;
        if (dealer == null || dealer.Side == Owner.Side || Owner.IsDead ||
            !command.Results.SelectMany(results => results).Any(result => result.Receiver == Owner)) return;
        await PowerCmd.Remove(this);
        var missiles = Owner.Player!.PlayerCombatState!.ExhaustPile.Cards.OfType<ShadowMissile>().ToArray();
        foreach (var missile in missiles)
        {
            if (!dealer.IsAlive || Owner.IsDead) break;
            missile.RuneTargetsAttacker = true;
            try { await CardCmd.AutoPlay(context, missile, dealer); }
            finally { missile.RuneTargetsAttacker = false; }
        }
    }
}
