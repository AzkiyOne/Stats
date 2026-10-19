using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.PlantDef;

public sealed class CanBeGrownInHydroponicsColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public CanBeGrownInHydroponicsColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.plant?.sowTags.Contains("Hydroponic") ?? false;
    }
}
