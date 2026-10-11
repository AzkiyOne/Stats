using Stats.Extensions;
using Verse;

namespace Stats.Tables;

public sealed class TurretDefTable : ThingDefTableWorker
{
    public TurretDefTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { building.IsTurret: true }
               && thingDef.IsBuildingObtainableByPlayer();
    }
}
