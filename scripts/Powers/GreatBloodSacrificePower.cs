using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class GreatBloodSacrificePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string CustomIconPath => "res://guestFromHeresy/images/cards/GreatBloodSacrifice.png";
    public override string CustomBigIconPath => CustomIconPath;

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType) =>
        card.Owner.Creature != Owner || card.Type == CardType.Attack;

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants) =>
        participants.Contains(Owner) ? PowerCmd.Remove(this) : Task.CompletedTask;
}
