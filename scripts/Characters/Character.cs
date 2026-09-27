using Godot;
using guestFromHeresy.Pools;
using MegaCrit.Sts2.Core.Entities.Characters;
using STS2RitsuLib.Data.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;

namespace guestFromHeresy.Characters;

[RegisterCharacter]
public class Character : ModCharacterTemplate<CardPool, RelicPool, PotionPool>
{
    public override Color NameColor => new(0f, 47f / 255f, 167f / 255f);
    public override Color EnergyLabelOutlineColor => NameColor;
    public override Color MapDrawingColor => NameColor;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 42;
    public override int StartingGold => 99;
    public override bool RequiresEpochAndTimeline => false;
    public override float AttackAnimDelay => 0f;
    public override float CastAnimDelay => 0f;

    public override List<string> GetArchitectAttackVfx() =>
    [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];

    // Use vanilla assets until the character's custom scenes and textures are ready.
    public override CharacterAssetProfile AssetProfile => CharacterAssetProfiles.Necrobinder();

    // Add StartingDeckEntries and StartingRelicTypes, or register starters with attributes.
}
