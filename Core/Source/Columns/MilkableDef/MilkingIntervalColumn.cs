using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;

namespace Stats.Columns.MilkableDef;

public sealed class MilkingIntervalColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberColumnCell>(columnDef)
        where TRecord :
            IMilkableDefTableRecord
{
    protected override NumberColumnCell MakeCell(TRecord record)
    {
        CompProperties_Milkable? milkableCompProps = record.MilkableCompProperties;

        if (milkableCompProps != null)
        {
            decimal milkIntervalDays = milkableCompProps.milkIntervalDays;

            return new NumberColumnCell(milkIntervalDays, "0 d");
        }

        return default;
    }
}
