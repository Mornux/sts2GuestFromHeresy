using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Relics;

public abstract class TemporaryIconRelic : ModRelicTemplate
{
    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"res://guestFromHeresy/images/relics/{GetType().Name}_temp.png",
        IconOutlinePath: $"res://guestFromHeresy/images/relics/{GetType().Name}_temp.png",
        BigIconPath: $"res://guestFromHeresy/images/relics/{GetType().Name}_temp.png");
}
