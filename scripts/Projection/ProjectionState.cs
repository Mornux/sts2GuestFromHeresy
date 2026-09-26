namespace guestFromHeresy.Projection;

public sealed class ProjectionState
{
    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }

    //Calculate starting maximum HP from growth stacks and create a fully healed projection.
    public ProjectionState(int readings)
    {
        MaxHp = 1 + 3 * readings;
        CurrentHp = MaxHp;
    }

    //Increase maximum and current HP together, ignoring nonpositive amounts.
    public void GainMaxHp(int amount)
    {
        if (amount <= 0) return;
        MaxHp += amount;
        CurrentHp += amount;
    }

    //Restore HP up to the maximum and return the amount actually healed.
    public int Heal(int amount)
    {
        int healed = Math.Min(Math.Max(amount, 0), MaxHp - CurrentHp);
        CurrentHp += healed;
        return healed;
    }

    //Truncate nonnegative damage to an integer, discard overflow, and return actual HP lost.
    public int TakeDamage(decimal amount)
    {
        int lost = (int)Math.Min(CurrentHp, Math.Max(0, decimal.Truncate(amount)));
        CurrentHp -= lost;
        return lost;
    }
}
