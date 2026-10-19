using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.AnimalDef;

public sealed class IsPackAnimalColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public IsPackAnimalColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.race?.packAnimal ?? false;
    }
}
