using System.Collections.Generic;
using Stats.TableRecords;

namespace Stats.Columns.ThingDef;

public sealed class HasInteractionCellColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public HasInteractionCellColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool AutoRefresh => false;

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.hasInteractionCell;
    }
}
