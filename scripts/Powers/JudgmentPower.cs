using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class JudgmentPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomIconPath => "res://guestFromHeresy/images/powers/Judgment_temp.png";
    public override string CustomBigIconPath => CustomIconPath;

    public async Task Advance()
    {
        if (Owner.IsDead) return;
        if (Amount > 1) await PowerCmd.Decrement(this);
        else
        {
            await PowerCmd.Remove(this);
            await CreatureCmd.Kill(Owner, force: true);
        }
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player) await Advance();
    }
}
