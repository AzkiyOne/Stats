using RimWorld;
using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.BedDef;

public sealed class FitsLargeAnimalsColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public FitsLargeAnimalsColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    protected override bool GetValueFromRecord(TRecord record)
    {
        BuildingProperties? buildingProperties = record.ThingDef.building;

        return buildingProperties is { bed_humanlike: false, bed_maxBodySize: > 0.55f };
    }
}
