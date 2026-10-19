using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.PlantDef;

public sealed class IsBlightableColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public IsBlightableColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.plant?.Blightable ?? false;
    }
}
