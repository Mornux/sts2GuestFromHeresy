using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

public abstract class TemporaryArtSkill(int cost, CardRarity rarity, TargetType target = TargetType.Self)
    : ModCardTemplate(cost, CardType.Skill, rarity, target, true)
{
    protected virtual string PortraitName => "TempImage";

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://guestFromHeresy/images/cards/{PortraitName}.png",
        BetaPortraitPath: $"res://guestFromHeresy/images/cards/{PortraitName}.png");
}
