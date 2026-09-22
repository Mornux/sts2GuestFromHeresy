using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Relics;

[RegisterRelic(typeof(RelicPool))]
public sealed class VoidFragment : ModRelicTemplate
{
    private int _stacks = 1;

    public override RelicRarity Rarity => RelicRarity.Event;
    public override bool ShowCounter => true;
    public override int DisplayAmount => Stacks;

    [SavedProperty]
    public int Stacks
    {
        get => _stacks;
        set
        {
            AssertMutable();
            _stacks = Math.Max(1, value);
            InvokeDisplayAmountChanged();
        }
    }

    public override RelicAssetProfile AssetProfile => new(
        IconPath: "res://guestFromHeresy/images/projection/ancient_tome.svg",
        IconOutlinePath: "res://guestFromHeresy/images/projection/ancient_tome.svg",
        BigIconPath: "res://guestFromHeresy/images/projection/ancient_tome.svg");
}
