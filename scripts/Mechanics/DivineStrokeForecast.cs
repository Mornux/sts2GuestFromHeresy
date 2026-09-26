namespace guestFromHeresy.Mechanics;

internal enum DivineStrokeOption { Draw, Block, HpLoss }

// Pure forecast state: evaluating a choice must never change the real combat.
internal sealed class DivineStrokeForecast(decimal block, int projectionHp)
{
    public decimal BodyHpLoss { get; private set; }
    public void ReceiveHit(decimal damage)
    {
        damage = Math.Max(0m, decimal.Truncate(damage));
        decimal unblocked = Math.Max(0m, damage - block);
        block = Math.Max(0m, block - damage);
        if (projectionHp > 0)
        {
            // Matches ProjectionState.TakeDamage: the breaking hit's overflow is discarded.
            projectionHp -= (int)Math.Min(projectionHp, unblocked);
            return;
        }
        BodyHpLoss += unblocked;
    }

    public static bool CanKeepDrawing(int handCount, int energy, bool drawAllowed = true) => drawAllowed && handCount < 2 && energy > 0;
    public static DivineStrokeOption Choose(int handCount, int energy, decimal lossAfterHpLoss, decimal lossAfterBlock, bool drawAllowed = true) =>
        CanKeepDrawing(handCount, energy, drawAllowed) ? DivineStrokeOption.Draw :
        lossAfterHpLoss > lossAfterBlock ? DivineStrokeOption.Block : DivineStrokeOption.HpLoss;
}
