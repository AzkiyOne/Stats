using System.Collections.Generic;
using Stats.TableRecords;

namespace Stats.Columns.BedDef;

public sealed class FitsSmallAnimalsColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public FitsSmallAnimalsColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool AutoRefresh => false;

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.building?.bed_humanlike == false;
    }
}
