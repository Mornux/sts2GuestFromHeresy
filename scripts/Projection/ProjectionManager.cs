using System.Runtime.CompilerServices;
using guestFromHeresy.Powers;
using guestFromHeresy.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;

namespace guestFromHeresy.Projection;

public static class ProjectionManager
{
    private static readonly ConditionalWeakTable<Player, CombatProjection> States = new();
    
    //Read Void Fragment stacks as projection growth, returning 0 when absent.
    public static int Readings(Player player) => player.GetRelic<VoidFragment>()?.Stacks ?? 0;
    
    
    //Obtain Void Fragment on the first reading, or add one stack and flash the existing relic.
    public static async Task ReadTome(Player player)
    {
        var relic = player.GetRelic<VoidFragment>();
        if (relic == null)
        {
            await RelicCmd.Obtain<VoidFragment>(player);
        }
        else
        {
            relic.Stacks++;
            relic.Flash();
        }
    }
    
    
    //Check whether the player uses this mod's character; null players are unsupported.
    public static bool Supports(Player? player) => player?.Character is Characters.Character;

    
    //Get the player's projection, recreating it from growth stacks on first access or combat changes.
    public static ProjectionState Get(Player player)
    {
        var holder = States.GetOrCreateValue(player);
        if (holder.State == null || !ReferenceEquals(holder.Combat, player.Creature.CombatState))
        {
            holder.Combat = player.Creature.CombatState;
            holder.State = new ProjectionState(Readings(player));
        }
        return holder.State;
    }

    
    //Treat Gazed as active only when its amount and the projection's current HP are positive.
    public static bool IsGazed(Player player) => player.Creature.GetPowerAmount<GazedPower>() > 0
        && Get(player).CurrentHp > 0;

    
    //Increase the player's current projection maximum and current HP by the same amount.
    public static void ProjectionGainMaxHp(Player player, int amount) => Get(player).GainMaxHp(amount);
    //Heal the player's current projection and return the HP restored without applying Gazed.
    public static int Heal(Player player, int amount) => Get(player).Heal(amount);

    
    private sealed class CombatProjection
    {
        public ICombatState? Combat;
        public ProjectionState? State;
    }
}
