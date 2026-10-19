using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.AnimalDef;

public sealed class IsBlockedByFencesColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public IsBlockedByFencesColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.race?.FenceBlocked ?? false;
    }
}
