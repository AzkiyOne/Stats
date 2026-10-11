using RimWorld;
using Stats.Extensions;
using Verse;

namespace Stats.Tables;

public sealed class RecreationalBuildingDefTable : ThingDefTableWorker
{
    public RecreationalBuildingDefTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsBuildingObtainableByPlayer()
               && thingDef.statBases?.GetStatValueFromList(StatDefOf.JoyGainFactor, 0f) > 0f;
    }
}
