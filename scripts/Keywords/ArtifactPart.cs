using guestFromHeresy.Scripts;
using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace guestFromHeresy.Keywords;

[RegisterOwnedKeyword("ArtifactPart", TitleTable = "card_keywords", TitleKey = "ARTIFACT_PART.title",
    DescriptionTable = "card_keywords", DescriptionKey = "ARTIFACT_PART.description", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
public sealed class ArtifactPart
{
    public static string Id => ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, "ArtifactPart");
    public static CardKeyword Value => ModKeywordRegistry.Get(Id).CardKeywordValue;
}
