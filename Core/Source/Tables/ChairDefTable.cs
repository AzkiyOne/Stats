using Stats.Extensions;
using Verse;

namespace Stats.Tables;

public sealed class ChairDefTable : ThingDefTableWorker
{
    public ChairDefTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building is { isSittable: true } && thingDef.IsBuildingObtainableByPlayer();
    }
}
