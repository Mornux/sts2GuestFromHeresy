using Godot;
using guestFromHeresy.Cards;
using guestFromHeresy.Capabilities;
using guestFromHeresy.Projection;
using guestFromHeresy.Scripts;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Combat;
using STS2RitsuLib.Models.Capabilities;

namespace guestFromHeresy.Keywords;

[RegisterOwnedKeyword("Synaesthesia", TitleTable = "card_keywords",
    TitleKey = "SYNAESTHESIA.title", DescriptionTable = "card_keywords",
    DescriptionKey = "SYNAESTHESIA.description")]
public sealed class Synaesthesia
{
    public static string Id => ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, "Synaesthesia");

    //Activate Synaesthesia while Gazed in combat.
    public static bool IsActive(CardModel card) => card.IsMutable && card.Owner?.Creature.CombatState != null
        && ProjectionManager.IsGazed(card.Owner);

    //Use the native playable outline and refresh its color as activation changes.
    public static void RegisterOutline() => ModContentRegistry.For(Entry.ModId)
        .RegisterCardHandOutline<CardModel>(card => CanEnhance(card) && card.IsOwnerPlayPhase() && card.CanPlay()
            ? new Color(237f / 255f, 200f / 255f, 125f / 255f) : null,
            visibleWhenUnplayable: false);

    private static bool CanEnhance(CardModel card) =>
        card is BreakFree escape && escape.CanGainExtraBlock
        || IsActive(card) && (card is Echolalia or InnerFear or ShadowStitching or GloomLash or ReversePosition
            or HatredField or NightmareFuel or AscendingLaw or SynaestheticAttack
            || card.Capability<SynaestheticGaze>() != null || card.Capability<SynaestheticDamage>() != null || card.Capability<SynaestheticFreeCost>() != null);
}
