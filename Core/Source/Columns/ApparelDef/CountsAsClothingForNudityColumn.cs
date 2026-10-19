using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.ApparelDef;

public sealed class CountsAsClothingForNudityColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public CountsAsClothingForNudityColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.apparel?.countsAsClothingForNudity ?? false;
    }
}
