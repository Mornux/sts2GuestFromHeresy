using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Runtime.CompilerServices;

namespace guestFromHeresy.Powers;

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
            var results = await CreatureCmd.Damage(context, owner, layers, ValueProp.Unpowered, owner, null);
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
