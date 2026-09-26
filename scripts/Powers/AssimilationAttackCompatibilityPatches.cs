using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace guestFromHeresy.Powers;

[HarmonyPatch(typeof(SkittishPower), nameof(SkittishPower.AfterAttack))]
internal static class AssimilationSkittishAttackPatch
{
    private static readonly MethodInfo SetTriggered = AccessTools.PropertySetter(typeof(SkittishPower), nameof(SkittishPower.HasGainedBlockThisTurn));
    internal static bool WasHit(PowerModel power, AttackCommand command) => command.DamageProps.HasFlag(ValueProp.Move)
        && command.Results.SelectMany(results => results).Any(result => result.Receiver == power.Owner && result.UnblockedDamage > 0);
    private static bool Prefix(SkittishPower __instance, AttackCommand command, ref Task __result)
    {
        if (!__instance.Owner.IsPlayer) return true;
        __result = Task.CompletedTask;
        if (__instance.Owner.IsAlive && !__instance.HasGainedBlockThisTurn && WasHit(__instance, command))
        {
            SetTriggered.Invoke(__instance, [true]);
            __result = CreatureCmd.GainBlock(__instance.Owner, __instance.Amount, ValueProp.Unpowered, null);
        }
        return false;
    }
    internal static void Reset(SkittishPower power) => SetTriggered.Invoke(power, [false]);
}

[HarmonyPatch(typeof(SkittishPower), nameof(SkittishPower.AfterSideTurnEnd))]
internal static class AssimilationSkittishResetPatch
{
    private static bool Prefix(SkittishPower __instance, CombatSide side, ref Task __result)
    {
        if (!__instance.Owner.IsPlayer) return true;
        if (side != __instance.Owner.Side) AssimilationSkittishAttackPatch.Reset(__instance);
        __result = Task.CompletedTask;
        return false;
    }
}

// Curl Up's native trigger waits for a player card to finish. Player owners instead
// resolve after the incoming attack, so a multi-hit attack still grants block once.
[HarmonyPatch(typeof(CurlUpPower), nameof(CurlUpPower.AfterDamageReceived))]
internal static class AssimilationCurlUpReceivePatch
{
    private static bool Prefix(CurlUpPower __instance, ref Task __result)
    {
        if (!__instance.Owner.IsPlayer) return true;
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterAttack))]
internal static class AssimilationCurlUpAttackPatch
{
    private static bool Prefix(AbstractModel __instance, AttackCommand command, ref Task __result)
    {
        if (__instance is not CurlUpPower power || !power.Owner.IsPlayer) return true;
        __result = power.Owner.IsAlive && command.DamageProps.IsPoweredAttack()
            && command.Results.SelectMany(results => results).Any(result => result.Receiver == power.Owner)
            ? Resolve(power) : Task.CompletedTask;
        return false;
    }
    private static async Task Resolve(PowerModel power)
    {
        await CreatureCmd.GainBlock(power.Owner, power.Amount, ValueProp.Unpowered, null);
        await PowerCmd.Remove(power);
    }
}
