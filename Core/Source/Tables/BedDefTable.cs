using Stats.Extensions;
using Verse;

namespace Stats.Tables;

public sealed class BedDefTable : ThingDefTableWorker
{
    public BedDefTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building != null && thingDef.IsBuildingObtainableByPlayer() && thingDef.IsBed;
    }
}
