using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.BedDef;

public sealed class FitsSmallAnimalsColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public FitsSmallAnimalsColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.building?.bed_humanlike == false;
    }
}
