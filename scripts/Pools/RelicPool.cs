using guestFromHeresy.Scripts;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.Pools;

public class RelicPool : TypeListRelicPoolModel
{
    public override string EnergyColorName => Entry.ModId;
    public override string? TextEnergyIconPath => "res://guestFromHeresy/images/signals/energy.png";
    public override string? BigEnergyIconPath => "res://guestFromHeresy/images/signals/energy.png";
}
