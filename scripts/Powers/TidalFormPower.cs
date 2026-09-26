using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using guestFromHeresy.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace guestFromHeresy.Powers;

[RegisterPower]
public sealed class TidalFormPower : TemporaryIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount) =>
        card.Owner == Owner.Player && card.Tags.Contains(CardTags.MoonPhase) ? playCount + Amount : playCount;
}
