using Stats.Extensions;
using Verse;

namespace Stats.Tables;

public sealed class ArtBuildingDefTable : ThingDefTableWorker
{
    public ArtBuildingDefTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building != null && thingDef.IsBuildingObtainableByPlayer() && thingDef.IsArt;
    }
}
