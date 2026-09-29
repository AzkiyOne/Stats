using System.Collections.Generic;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class IsPackAnimalColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public IsPackAnimalColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool IsRefreshable => false;

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.race?.packAnimal ?? false;
    }
}
