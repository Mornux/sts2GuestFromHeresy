using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace guestFromHeresy.Powers;

[HarmonyPatch]
public static class GravityMomentDamagePatch
{
    public static MethodBase TargetMethod()
    {
        var method = AccessTools.Method(typeof(CreatureCmd), nameof(CreatureCmd.Damage),
            [typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(MegaCrit.Sts2.Core.Entities.Cards.CardPlay)]);
        return AccessTools.Method(method.GetCustomAttribute<AsyncStateMachineAttribute>()!.StateMachineType, "MoveNext");
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var original = AccessTools.Method(typeof(Hook), nameof(Hook.ModifyDamage));
        var replacement = AccessTools.Method(typeof(GravityMomentDamagePatch), nameof(ModifyDamage));
        int matches = 0;
        foreach (var instruction in instructions)
        {
            if (instruction.Calls(original)) { instruction.opcode = OpCodes.Call; instruction.operand = replacement; matches++; }
            yield return instruction;
        }
        if (matches != 1) throw new InvalidOperationException("Gravity Moment damage integration requires review for this game version.");
    }

    public static decimal ModifyDamage(IRunState runState, ICombatState? combatState, Creature? target, Creature? dealer,
        decimal damage, ValueProp props, CardModel? cardSource, CardPlay? cardPlay, ModifyDamageHookType hookType, CardPreviewMode previewMode,
        out IEnumerable<AbstractModel> modifiers)
    {
        decimal modified = Hook.ModifyDamage(runState, combatState, target, dealer, damage, props, cardSource, cardPlay, hookType, previewMode, out modifiers);
        return target?.GetPower<GravityMomentPower>() is { } power ? power.Redirect(modified, props, dealer) : modified;
    }
}
