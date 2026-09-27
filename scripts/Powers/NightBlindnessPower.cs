using guestFromHeresy.Cards;
using guestFromHeresy.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
namespace guestFromHeresy.Powers;
[RegisterPower]
public sealed class NightBlindnessPower : TemporaryIconPower
{
    private bool _triggered;
    public override Task BeforeSideTurnStart(PlayerChoiceContext context, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combat)
    {
        if (participants.Contains(Owner)) _triggered = false;
        return Task.CompletedTask;
    }
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play)
    {
        if (_triggered || play.Card.Owner.Creature != Owner || !play.Card.Tags.Contains(CardTags.MoonPhase) || !Owner.IsAlive) return;
        _triggered = true;
        await CardPileCmd.Draw(context, Amount, Owner.Player!);
    }
}
