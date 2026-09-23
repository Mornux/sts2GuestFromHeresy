using guestFromHeresy.Pools;
using guestFromHeresy.Projection;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Relics;

[RegisterRelic(typeof(RelicPool))]
[RegisterCharacterStarterRelic(typeof(Characters.Character))]
[RegisterTouchOfOrobasRefinement(typeof(UnsealedPortal))]
public sealed class SealedGate : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override RelicAssetProfile AssetProfile => new(
        IconPath: "res://guestFromHeresy/images/projection/SealedGate_temp.png",
        IconOutlinePath: "res://guestFromHeresy/images/projection/SealedGate_temp.png",
        BigIconPath: "res://guestFromHeresy/images/projection/SealedGate_temp.png");

    public override Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner == Owner && amount > 0)
        {
            ProjectionManager.ProjectionGainMaxHp(Owner, amount);
            Flash();
        }
        return Task.CompletedTask;
    }
}