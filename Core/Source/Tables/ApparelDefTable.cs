using Verse;

namespace Stats.Tables;

// We do not check for "destroyOnDrop" for better compatibility with mods like
// VFE - Pirates.
public sealed class ApparelDefTable : ThingDefTableWorker
{
    public ApparelDefTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.apparel != null;
    }
}
