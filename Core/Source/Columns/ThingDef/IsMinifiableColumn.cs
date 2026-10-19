using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.ThingDef;

public sealed class IsMinifiableColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public IsMinifiableColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.Minifiable;
    }
}
