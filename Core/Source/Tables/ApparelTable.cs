using Verse;

namespace Stats.Tables;

public sealed class ApparelTable : ThingTableWorker
{
    public ApparelTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.apparel != null;
    }

    protected override bool IsValidThing(Thing thing)
    {
        return IsValidThingDef(thing.def);
    }
}
