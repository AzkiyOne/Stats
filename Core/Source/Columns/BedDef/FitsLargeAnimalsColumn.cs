using System.Collections.Generic;
using RimWorld;
using Stats.TableRecords;

namespace Stats.Columns.BedDef;

public sealed class FitsLargeAnimalsColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public FitsLargeAnimalsColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool AutoRefresh => false;

    protected override bool GetValueFromRecord(TRecord record)
    {
        BuildingProperties? buildingProperties = record.ThingDef.building;

        return buildingProperties is { bed_humanlike: false, bed_maxBodySize: > 0.55f };
    }
}
