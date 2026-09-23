using guestFromHeresy.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class DeadlinePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomIconPath => "res://guestFromHeresy/images/powers/Deadline_temp.png";
    public override string CustomBigIconPath => CustomIconPath;

    public void Reset() => SetAmount(13);

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || Owner.IsDead) return;
        if (cardPlay.Card is ApproachingDeadline)
        {
            Reset();
            return;
        }
        if (Amount > 1) await PowerCmd.Decrement(this);
        else
        {
            SetAmount(0);
            await CreatureCmd.Kill(Owner, force: true);
        }
    }
}
