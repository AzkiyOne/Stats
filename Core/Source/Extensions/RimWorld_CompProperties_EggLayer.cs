using RimWorld;
using Verse;

namespace Stats.Extensions;

public static class RimWorld_CompProperties_EggLayer
{
    public static ThingDef GetAnyEggDef(this CompProperties_EggLayer compProps)
    {
        return compProps.eggUnfertilizedDef ?? compProps.eggFertilizedDef;
    }
}
