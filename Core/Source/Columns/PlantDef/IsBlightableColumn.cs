using System.Collections.Generic;
using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class IsBlightableColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public IsBlightableColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool AutoRefresh => false;

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.plant?.Blightable ?? false;
    }
}
