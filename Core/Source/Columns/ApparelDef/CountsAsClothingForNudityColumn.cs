using Stats.TableRecords;

namespace Stats.Columns.ApparelDef;

public sealed class CountsAsClothingForNudityColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IApparelDefTableRecord
{
    public CountsAsClothingForNudityColumn(ColumnDef columnDef) : base(columnDef)
    {
    }

    protected override bool GetValue(TRecord record)
    {
        return record.ApparelProperties.countsAsClothingForNudity;
    }
}
