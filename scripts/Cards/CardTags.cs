using guestFromHeresy.Scripts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.CardTags;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace guestFromHeresy.Cards;

[RegisterOwnedCardTag("MoonPhase")]
public sealed class CardTags
{
    public static CardTag MoonPhase => ModCardTagExtensions.GetModCardTag(
        ModContentRegistry.GetQualifiedCardTagId(Entry.ModId, "MoonPhase"));
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
internal static class MoonPhaseHoverTipsPatch
{
    private static void Postfix(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance.Tags.Contains(CardTags.MoonPhase))
            __result = __result.Append(new HoverTip(
                new LocString("card_keywords", "MOON_PHASE.title"),
                new LocString("card_keywords", "MOON_PHASE.description")));
    }
}
