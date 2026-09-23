using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Cards;

public abstract class TemporaryArtPower(int cost, CardRarity rarity, TargetType target = TargetType.Self)
    : ModCardTemplate(cost, CardType.Power, rarity, target, true)
{
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://guestFromHeresy/images/cards/TempImage.png",
        BetaPortraitPath: "res://guestFromHeresy/images/cards/TempImage.png");
}
