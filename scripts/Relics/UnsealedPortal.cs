using guestFromHeresy.Pools;
using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Relics;

[RegisterRelic(typeof(RelicPool))]
public sealed class UnsealedPortal : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    public override RelicAssetProfile AssetProfile => new(
        IconPath: "res://guestFromHeresy/images/projection/UnsealedPortal_temp.png",
        IconOutlinePath: "res://guestFromHeresy/images/projection/UnsealedPortal_temp.png",
        BigIconPath: "res://guestFromHeresy/images/projection/UnsealedPortal_temp.png");

    public override Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner == Owner && amount > 0)
        {
            ProjectionManager.ProjectionGainMaxHp(Owner, 2 * amount);
            Flash();
        }
        return Task.CompletedTask;
    }
}
