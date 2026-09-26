using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace guestFromHeresy.Powers;

// Keep the native power identity, icon and stacks. Only monster-specific actions
// are disabled on players; powers owned by enemies continue to use their native code.
[HarmonyPatch]
internal static class AssimilationMonsterActionPatch
{
    internal static IEnumerable<MethodBase> TargetMethods()
    {
        (Type Type, string[] Methods)[] hooks =
        [
            (typeof(AdaptablePower), [nameof(PowerModel.AfterDeath)]),
            (typeof(IllusionPower), [nameof(PowerModel.AfterApplied), nameof(PowerModel.AfterDeath), nameof(PowerModel.AfterCombatEnd)]),
            (typeof(InfestedPower), [nameof(PowerModel.AfterDeath)]),
            (typeof(RampartPower), [nameof(PowerModel.AfterSideTurnStart)]),
            (typeof(ReattachPower), [nameof(PowerModel.AfterDeath), nameof(ReattachPower.DoReattach)]),
            (typeof(SandpitPower), [nameof(PowerModel.AfterApplied), nameof(PowerModel.AfterSideTurnStartLate),
                nameof(PowerModel.AfterPowerAmountChanged), nameof(PowerModel.AfterRemoved),
                nameof(PowerModel.AfterCreatureAddedToCombat), nameof(PowerModel.AfterOstyRevived), nameof(PowerModel.BeforeSideTurnEnd)]),
            (typeof(SlumberPower), [nameof(PowerModel.AfterRemoved)]),
            (typeof(SteamEruptionPower), [nameof(PowerModel.AfterDeath)]),
            (typeof(StockPower), [nameof(PowerModel.AfterDeath)]),
            (typeof(SurprisePower), [nameof(PowerModel.AfterDeath)]),
            (typeof(HeistPower), [nameof(PowerModel.BeforeDeath)]),
            (typeof(SwipePower), [nameof(PowerModel.BeforeDeath), nameof(SwipePower.Steal)]),
            (typeof(ThieveryPower), [nameof(ThieveryPower.Steal)]),
            (typeof(WitheringPresencePower), [nameof(PowerModel.AfterCardPlayed)])
        ];
        foreach (var (type, methods) in hooks)
            foreach (string name in methods)
                yield return AccessTools.DeclaredMethod(type, name)
                    ?? throw new MissingMethodException(type.FullName, name);
    }

    private static bool Prefix(PowerModel __instance, ref Task __result)
    {
        if (!__instance.Owner.IsPlayer) return true;
        __result = Task.CompletedTask;
        return false;
    }
}

// Monster resurrection powers must not keep a completed player battle alive,
// or suppress the normal death/cleanup of unrelated creatures and powers.
[HarmonyPatch]
internal static class AssimilationMonsterDeathRulesPatch
{
    internal static IEnumerable<MethodBase> TargetMethods()
    {
        Type[] types = [typeof(AdaptablePower), typeof(IllusionPower), typeof(InfestedPower),
            typeof(ReattachPower), typeof(SteamEruptionPower), typeof(StockPower), typeof(SurprisePower)];
        string[] methods = [nameof(PowerModel.ShouldStopCombatFromEnding),
            nameof(PowerModel.ShouldCreatureBeRemovedFromCombatAfterDeath),
            nameof(PowerModel.ShouldPowerBeRemovedAfterOwnerDeath), nameof(PowerModel.ShouldPowerBeRemovedOnDeath),
            nameof(PowerModel.ShouldOwnerDeathTriggerFatal)];
        foreach (Type type in types)
            foreach (string name in methods)
                if (AccessTools.DeclaredMethod(type, name) is { } method) yield return method;
    }

    private static bool Prefix(PowerModel __instance, MethodBase __originalMethod, ref bool __result)
    {
        if (!__instance.Owner.IsPlayer) return true;
        __result = __originalMethod.Name != nameof(PowerModel.ShouldStopCombatFromEnding);
        return false;
    }
}

// These powers mix a usable counter/damage reduction with a monster wake/stun animation.
[HarmonyPatch]
internal static class AssimilationWakeOnDamagePatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        new[] { typeof(AsleepPower), typeof(SlumberPower), typeof(FlutterPower) }
            .Select(type => AccessTools.DeclaredMethod(type, nameof(PowerModel.AfterDamageReceived)));

    private static bool Prefix(PowerModel __instance, Creature target, DamageResult result, ValueProp props, ref Task __result)
    {
        if (!__instance.Owner.IsPlayer) return true;
        __result = target == __instance.Owner && result.UnblockedDamage > 0
            && (__instance is not FlutterPower || props.IsPoweredAttack())
            ? Wake(__instance) : Task.CompletedTask;
        return false;
    }

    private static async Task Wake(PowerModel power)
    {
        if (power is AsleepPower)
        {
            if (power.Owner.GetPower<PlatingPower>() is { } plating) await PowerCmd.Remove(plating);
            await PowerCmd.Remove(power);
        }
        else await PowerCmd.Decrement(power);
    }
}

[HarmonyPatch]
internal static class AssimilationSleepCountdownPatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        new[] { typeof(AsleepPower), typeof(SlumberPower) }
            .Select(type => AccessTools.DeclaredMethod(type, nameof(PowerModel.AfterSideTurnEnd)));

    private static bool Prefix(PowerModel __instance, IEnumerable<Creature> participants, ref Task __result)
    {
        if (!__instance.Owner.IsPlayer) return true;
        __result = participants.Contains(__instance.Owner) ? PowerCmd.Decrement(__instance) : Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(RavenousPower), nameof(RavenousPower.AfterDeath))]
internal static class AssimilationRavenousPatch
{
    private static bool Prefix(RavenousPower __instance,
        MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext choiceContext,
        Creature target, bool wasRemovalPrevented, ref Task __result)
    {
        if (!__instance.Owner.IsPlayer) return true;
        __result = !wasRemovalPrevented && target != __instance.Owner && target.Side == __instance.Owner.Side && __instance.Owner.IsAlive
            ? PowerCmd.Apply<StrengthPower>(choiceContext, __instance.Owner, __instance.Amount, __instance.Owner, null)
            : Task.CompletedTask;
        return false;
    }
}
