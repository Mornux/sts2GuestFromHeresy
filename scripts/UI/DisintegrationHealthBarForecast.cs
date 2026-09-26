using Godot;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Combat.HealthBars;

namespace guestFromHeresy.UI;

public sealed class DisintegrationHealthBarForecast : IHealthBarForecastSource
{
    private static readonly Color OverlayColor = new(21f / 255f, 46f / 255f, 56f / 255f);

    public IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        var creature = context.Creature;
        int amount = Math.Max(0, creature.GetPowerAmount<DisintegrationPower>() - creature.Block);
        return HealthBarForecasts.Single(amount, OverlayColor, HealthBarForecastGrowthDirection.FromRight);
    }
}
