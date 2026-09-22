using guestFromHeresy.Scripts;
using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.CardTags;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterOwnedCardTag("MoonPhase")]
public static class CardTags
{
    public static CardTag MoonPhase => ModCardTagExtensions.GetModCardTag(
        ModContentRegistry.GetQualifiedCardTagId(Entry.ModId, "MoonPhase"));
}
