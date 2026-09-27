using guestFromHeresy.Scripts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.CardTags;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterOwnedCardTag("MoonPhase")]
[RegisterOwnedCardTag("Artifact")]
public sealed class CardTags
{
    public static IHoverTip ArtifactHoverTip => new HoverTip(
        new LocString("card_keywords", "SEALED_ARTIFACT.title"),
        new LocString("card_keywords", "SEALED_ARTIFACT.description"));

    public static CardTag Artifact => ModCardTagExtensions.GetModCardTag(
        ModContentRegistry.GetQualifiedCardTagId(Entry.ModId, "Artifact"));

    public static IHoverTip MoonPhaseHoverTip => new HoverTip(
        new LocString("card_keywords", "MOON_PHASE.title"),
        new LocString("card_keywords", "MOON_PHASE.description"));

    public static CardTag MoonPhase => ModCardTagExtensions.GetModCardTag(
        ModContentRegistry.GetQualifiedCardTagId(Entry.ModId, "MoonPhase"));
}

