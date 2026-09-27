using System.Collections.Generic;
using Stats.TableRecords;

namespace Stats.Columns.ThingDef;

public sealed class IsMinifiableColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public IsMinifiableColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool IsRefreshable => false;

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.Minifiable;
    }
}
