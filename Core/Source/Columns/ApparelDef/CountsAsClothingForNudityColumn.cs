using System.Collections.Generic;
using Stats.TableRecords;

namespace Stats.Columns.ApparelDef;

public sealed class CountsAsClothingForNudityColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public CountsAsClothingForNudityColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool AutoRefresh => false;

    protected override bool GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.apparel?.countsAsClothingForNudity ?? false;
    }
}
