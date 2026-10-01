using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Runtime.CompilerServices;

namespace guestFromHeresy.Powers;

[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Models.PowerModel), nameof(MegaCrit.Sts2.Core.Models.PowerModel.Description), MethodType.Getter)]
internal static class DisintegrationDescriptionPatch
{
    private static void Postfix(MegaCrit.Sts2.Core.Models.PowerModel __instance,
        ref MegaCrit.Sts2.Core.Localization.LocString __result)
    {
        // Canonical keyword tips have no owning creature/stack count. Live powers keep
        // the vanilla smartDescription, which already formats their current Amount.
        if (__instance is DisintegrationPower)
            __result = new("powers", "GUEST_FROM_HERESY_DISINTEGRATION.description");
    }
}

public static class DisintegrationTriggers
{
    private static readonly ConditionalWeakTable<ICombatState, Dictionary<Creature, int>> TriggerCounts = new();
    public static int TimesTriggered(Creature? creature) => creature?.CombatState is { } combat
        && TriggerCounts.TryGetValue(combat, out var counts) ? counts.GetValueOrDefault(creature) : 0;

    public static async Task<int> Trigger(PlayerChoiceContext context, DisintegrationPower power)
    {
        var owner = power.Owner;
        var combat = owner.CombatState;
        if (combat == null) return 0;
        int hpLost = 0;
        int repeats = 1 + combat.Creatures.Where(c => c.IsAlive).Sum(c => c.GetPowerAmount<HatefirePower>());
        for (int i = 0; i < repeats && owner.IsAlive && power.Amount > 0; i++)
        {
            int layers = power.Amount;
            var counts = TriggerCounts.GetOrCreateValue(combat);
            counts[owner] = counts.GetValueOrDefault(owner) + 1;
            var counters = combat.Creatures.Where(c => c.IsAlive).SelectMany(c => c.Powers.OfType<PerpetualDecayPower>()).ToArray();
            var results = await CreatureCmd.Damage(context, owner, layers, ValueProp.Unpowered, owner, null, null);
            hpLost += results.Where(result => result.Receiver == owner).Sum(result => result.UnblockedDamage);
            foreach (var counter in counters) await counter.CountTrigger(context, layers);
        }
        return hpLost;
    }
}

[HarmonyPatch(typeof(DisintegrationPower), nameof(DisintegrationPower.AfterSideTurnEndLate))]
internal static class DisintegrationTriggerPatch
{
    private static bool Prefix(DisintegrationPower __instance, PlayerChoiceContext choiceContext,
        IEnumerable<Creature> participants, ref Task __result)
    {
        __result = participants.Contains(__instance.Owner) ? TriggerWithFx(choiceContext, __instance) : Task.CompletedTask;
        return false;
    }
    private static async Task TriggerWithFx(PlayerChoiceContext context, DisintegrationPower power)
    {
        await DisintegrationTriggers.Trigger(context, power);
        VfxCmd.PlayOnCreatureCenter(power.Owner, "vfx/vfx_attack_blunt");
    }
}
