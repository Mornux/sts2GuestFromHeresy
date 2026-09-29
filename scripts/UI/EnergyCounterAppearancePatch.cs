using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace guestFromHeresy.UI;

[HarmonyPatch(typeof(NEnergyCounter), nameof(NEnergyCounter.Create))]
public static class EnergyCounterAppearancePatch
{
    public static void Postfix(Player player, NEnergyCounter? __result)
    {
        if (__result == null || player.Character is not Characters.Character) return;

        // Counter scenes have different optional artwork layers. Missing artwork must
        // not interrupt combat setup; keep the native counter if its base layer differs.
        if (__result.GetNodeOrNull<TextureRect>("Layers/Layer1") is { } baseLayer)
        {
            var texture = ResourceLoader.Load<Texture2D>("res://guestFromHeresy/images/signals/energy.png");
            if (texture != null) baseLayer.Texture = texture;
        }
        __result.GetNodeOrNull<Control>("Layers/RotationLayers")?.Hide();
        __result.GetNodeOrNull<Control>("Layers/Layer4")?.Hide();
        __result.GetNodeOrNull<Control>("Layers/Layer5")?.Hide();
    }
}
