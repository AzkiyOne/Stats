using System.Collections.Generic;
using System.Linq;
using Stats.TableRecords;

namespace Stats.Columns.BuildingDef;

public sealed class RecreationTypeColumn<TRecord> : DefColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public RecreationTypeColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetJoyKindDefs(thingDefs))
    {
    }

    public override bool AutoRefresh => false;

    protected override Verse.Def? GetDef(TRecord record)
    {
        return record.ThingDef.building?.joyKind;
    }

    private static IEnumerable<Verse.Def?> GetJoyKindDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .Select(thingDef => thingDef.building?.joyKind)
            .Distinct();
    }
}
