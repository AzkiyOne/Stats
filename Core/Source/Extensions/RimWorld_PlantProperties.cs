using RimWorld;

namespace Stats.Extensions;

public static class RimWorld_PlantProperties
{
    public static float GetGrowDaysActual(this PlantProperties plantProperties)
    {
        // Source: Wiki
        return plantProperties.growDays / 0.5417f;
    }
}
