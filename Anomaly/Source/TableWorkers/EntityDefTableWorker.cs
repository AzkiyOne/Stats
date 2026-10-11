using Stats.Tables;
using Verse;

namespace Stats.Compat.Anomaly;

public sealed class EntityDefTableWorker : ThingDefTableWorker
{
    public EntityDefTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { race.IsAnomalyEntity: true, IsCorpse: false };
    }
}
