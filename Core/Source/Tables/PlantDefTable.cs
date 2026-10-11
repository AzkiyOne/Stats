using Verse;

namespace Stats.Tables;

public sealed class PlantDefTable : ThingDefTableWorker
{
    public PlantDefTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { IsPlant: true, plant.isStump: false };
    }
}
