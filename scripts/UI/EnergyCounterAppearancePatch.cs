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

        // The character uses the Ironclad counter scene. Replace its artwork before
        // _Ready runs, retaining the native label, darkening, and energy-gain effects.
        __result.GetNode<TextureRect>("Layers/Layer1").Texture =
            ResourceLoader.Load<Texture2D>("res://guestFromHeresy/images/signals/energy.png");
        __result.GetNode<Control>("Layers/RotationLayers").Hide();
        __result.GetNode<Control>("Layers/Layer4").Hide();
        __result.GetNode<Control>("Layers/Layer5").Hide();
    }
}
