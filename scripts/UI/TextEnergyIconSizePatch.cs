using System.Reflection;
using System.Reflection.Emit;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Text;
using STS2RitsuLib.Scaffolding.Content;

namespace guestFromHeresy.UI;

[HarmonyPatch]
public static class TextEnergyIconSizePatch
{
    internal const string IconPath = "res://guestFromHeresy/images/signals/energy.png";
    internal const int Width = 21;
    internal const int Height = 24;

    public static MethodBase TargetMethod() => AccessTools.Method(
        typeof(ModCardTemplate).Assembly.GetType(
            "STS2RitsuLib.Scaffolding.Content.Patches.ModTextEnergyIconHelper", throwOnError: true),
        "OverrideTextIconTag");

    public static void Postfix(ref string __result)
    {
        // Vanilla inline energy icons are 24px tall. Keep the 288x333 source's
        // aspect ratio without changing the artwork used by cards and the HUD.
        if (__result == $"[img]{IconPath}[/img]")
            __result = $"[img width={Width} height={Height}]{IconPath}[/img]";
    }
}

// MegaLabelHelper ignores [img] dimensions and measures the source texture.
// Match its estimate to our inline tag without resizing the texture used by the HUD.
[HarmonyPatch(typeof(MegaLabelHelper), nameof(MegaLabelHelper.EstimateTextSize),
    [typeof(TextParagraph), typeof(List<BbcodeObject>), typeof(Font), typeof(int), typeof(float), typeof(float)])]
public static class TextEnergyIconMeasurementPatch
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var original = AccessTools.Method(typeof(Texture2D), nameof(Texture2D.GetSize));
        var replacement = AccessTools.Method(typeof(TextEnergyIconMeasurementPatch), nameof(GetInlineSize));
        int matches = 0;
        foreach (var instruction in instructions)
        {
            if (instruction.Calls(original))
            {
                instruction.opcode = OpCodes.Call;
                instruction.operand = replacement;
                matches++;
            }
            yield return instruction;
        }
        if (matches != 1)
            throw new InvalidOperationException("Inline energy icon measurement requires review for this game version.");
    }

    public static Vector2 GetInlineSize(Texture2D texture) =>
        texture.ResourcePath == TextEnergyIconSizePatch.IconPath
            ? new Vector2(TextEnergyIconSizePatch.Width, TextEnergyIconSizePatch.Height)
            : texture.GetSize();
}
