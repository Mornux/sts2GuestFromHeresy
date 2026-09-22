using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using guestFromHeresy.Powers;
using guestFromHeresy.RestSites;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;

namespace guestFromHeresy.Projection;

[HarmonyPatch]
internal static class ProjectionDamagePatch
{
    //Locate the damage resolution async state machine's execution method as the patch target.
    private static MethodBase TargetMethod()
    {
        var method = AccessTools.Method(typeof(CreatureCmd), nameof(CreatureCmd.Damage),
            [typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal),
                typeof(ValueProp), typeof(Creature), typeof(CardModel)]);
        return AccessTools.Method(method.GetCustomAttribute<AsyncStateMachineAttribute>()!.StateMachineType, "MoveNext");
    }

    //Replace both actual HP-loss calls with projection handling and verify the expected call count.
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var original = AccessTools.Method(typeof(Creature), nameof(Creature.LoseHpInternal));
        var replacement = AccessTools.Method(typeof(ProjectionDamagePatch), nameof(LoseHp));
        int replaced = 0;
        foreach (var instruction in instructions)
        {
            if (instruction.Calls(original))
            {
                instruction.opcode = OpCodes.Call;
                instruction.operand = replacement;
                replaced++;
            }
            yield return instruction;
        }
        if (replaced != 2) throw new InvalidOperationException("Projection damage integration requires review for this game version.");
    }

    //Route Gazed damage to the projection, clear it on break and discard overflow; otherwise lose HP normally.
    public static DamageResult LoseHp(Creature creature, decimal amount, ValueProp props)
    {
        if (!ProjectionManager.Supports(creature.Player) || !ProjectionManager.IsGazed(creature.Player!))
            return creature.LoseHpInternal(amount, props);

        var state = ProjectionManager.Get(creature.Player!);
        int lost = state.TakeDamage(amount);
        if (state.CurrentHp == 0)
        {
            var gazed = creature.GetPower<GazedPower>()!;
            gazed.RemoveProjectionBonuses();
            gazed.SetAmount(0, true);
            gazed.RemoveInternal();
        }
        //Preserve hit triggers without reporting the real player dead or forwarding overkill.
        return new DamageResult(creature, props) { UnblockedDamage = lost };
    }
}

[HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.Apply),
    [typeof(PlayerChoiceContext), typeof(PowerModel), typeof(Creature), typeof(decimal),
        typeof(Creature), typeof(CardModel), typeof(bool)])]
internal static class ProjectionApplyPatch
{
    //Block Gazed on unsupported targets or zero-HP projections while allowing other powers.
    private static bool Prefix(PowerModel power, Creature target, ref Task __result)
    {
        if (power is not GazedPower || (ProjectionManager.Supports(target.Player)
            && ProjectionManager.Get(target.Player!).CurrentHp > 0)) return true;
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(PowerModel), nameof(PowerModel.RemoveInternal))]
internal static class ProjectionRemovePatch
{
    //Remove the projection's temporary stat bonuses before the Gazed power is removed.
    private static void Prefix(PowerModel __instance)
    {
        if (__instance is GazedPower gazed) gazed.RemoveProjectionBonuses();
    }
}

[HarmonyPatch(typeof(RestSiteOption), nameof(RestSiteOption.Generate))]
internal static class ProjectionRestSitePatch
{
    //Add Read Ancient Tome to this mod character's rest-site options without duplicates.
    private static void Postfix(Player player, List<RestSiteOption> __result)
    {
        if (ProjectionManager.Supports(player) && !__result.OfType<ReadAncientTome>().Any())
            __result.Add(new ReadAncientTome(player));
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature._Ready))]
internal static class ProjectionUiPatch
{
    //Attach a separate projection health bar after this mod character's creature node is ready.
    private static void Postfix(NCreature __instance)
    {
        if (ProjectionManager.Supports(__instance.Entity.Player))
            Godot.Callable.From(() =>
            {
                if (Godot.GodotObject.IsInstanceValid(__instance) && __instance.IsInsideTree())
                    __instance.AddChild(new ProjectionHealthBar { CreatureNode = __instance });
            }).CallDeferred();
    }
}
