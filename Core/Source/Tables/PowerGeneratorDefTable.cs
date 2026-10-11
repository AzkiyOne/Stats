using RimWorld;
using Verse;

namespace Stats.Tables;

public sealed class PowerGeneratorDefTable : ThingDefTableWorker
{
    public PowerGeneratorDefTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.GetCompProperties<CompProperties_Power>()?.PowerConsumption < 0f;
    }
}
