using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.PlantDef;

public sealed class IsDestroyedOnHarvestColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public IsDestroyedOnHarvestColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.plant?.HarvestDestroys ?? false;
    }
}
