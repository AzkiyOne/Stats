using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.PlantDef;

public sealed class CanBePlantedUnderRoofColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public CanBePlantedUnderRoofColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.plant?.interferesWithRoof == false;
    }
}
