using guestFromHeresy.Projection;
using guestFromHeresy.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.RestSites;

public sealed class ReadAncientTome(Player owner) : ModRestSiteOptionTemplate(owner)
{
    public override string OptionId => "GUEST_FROM_HERESY_READ_ANCIENT_TOME";
    public override string CustomIconPath => "res://guestFromHeresy/images/projection/AncientTome_temp.png";
    public override LocString Description => new("rest_site_ui",
        $"OPTION_{OptionId}.description");

    public override async Task<bool> OnSelect()
    {
        await ProjectionManager.ReadTome(Owner); // location: scripts/Projection/ProjectionManager.cs
        return true;
    }
}
