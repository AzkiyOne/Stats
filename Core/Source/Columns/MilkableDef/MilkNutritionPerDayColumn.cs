using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.MilkableDef;

public sealed class MilkNutritionPerDayColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public MilkNutritionPerDayColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.00/d")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_Milkable? milkableCompProps = record.ThingDef.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps is { milkDef: not null, milkIntervalDays: > 0 })
        {
            float milkNutrition = milkableCompProps.milkDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float milkAmount = milkableCompProps.milkAmount;
            float milkIntervalDays = milkableCompProps.milkIntervalDays;
            float milkPerDay = milkAmount / milkIntervalDays;
            float milkNutritionPerDay = milkPerDay * milkNutrition;

            return milkNutritionPerDay.ToDecimal(2);
        }

        return 0m;
    }
}
