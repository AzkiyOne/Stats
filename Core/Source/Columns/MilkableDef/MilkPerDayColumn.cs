using RimWorld;
using Stats.Columns.Cells;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.MilkableDef;

public sealed class MilkPerDayColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberColumnCell>(columnDef)
        where TRecord :
            IMilkableDefTableRecord
{
    protected override NumberColumnCell MakeCell(TRecord record)
    {
        CompProperties_Milkable? milkableCompProps = record.MilkableCompProperties;

        if (milkableCompProps is { milkIntervalDays: > 0 })
        {
            float milkAmount = milkableCompProps.milkAmount;
            float milkIntervalDays = milkableCompProps.milkIntervalDays;
            float milkPerDay = milkAmount / milkIntervalDays;

            return new NumberColumnCell(milkPerDay.ToDecimal(1), "0.0/d");
        }

        return default;
    }
}
