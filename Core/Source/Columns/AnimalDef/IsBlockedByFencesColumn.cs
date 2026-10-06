using System.Collections.Generic;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class IsBlockedByFencesColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public IsBlockedByFencesColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool AutoRefresh => false;

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.race?.FenceBlocked ?? false;
    }
}
