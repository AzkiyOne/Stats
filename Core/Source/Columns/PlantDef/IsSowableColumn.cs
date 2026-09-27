using System.Collections.Generic;
using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class IsSowableColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public IsSowableColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool IsRefreshable => false;

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.plant?.Sowable ?? false;
    }
}
