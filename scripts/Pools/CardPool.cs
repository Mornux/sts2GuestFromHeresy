using Godot;
using guestFromHeresy.Scripts;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace guestFromHeresy.Pools;

public class CardPool : TypeListCardPoolModel
{
    public override string Title => Entry.ModId;
    public override string EnergyColorName => Entry.ModId;
    public override string? TextEnergyIconPath => "res://guestFromHeresy/images/signals/energy.png";
    public override string? BigEnergyIconPath => "res://guestFromHeresy/images/signals/energy.png";
    public override Color DeckEntryCardColor => new(0f, 47f / 255f, 167f / 255f);
    public override Color EnergyOutlineColor => DeckEntryCardColor;
    public override bool IsColorless => false;

    private static readonly Material? CustomFrameMaterial =
        MaterialUtils.CreateReplaceHueShaderMaterial(0f, 47f / 255f, 167f / 255f);

    public override Material? PoolFrameMaterial => CustomFrameMaterial;
}
