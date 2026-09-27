using RimWorld;
using Stats.Columns.Cells;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class ProductsNutritionPerDayColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberColumnCell>(columnDef)
        where TRecord :
            IMilkableDefTableRecord,
            IEggLayerDefTableRecord
{
    protected override NumberColumnCell MakeCell(TRecord record)
    {
        // Milk
        float milkNutritionPerDay = 0f;
        CompProperties_Milkable? milkableCompProps = record.MilkableCompProperties;

        if (milkableCompProps is { milkDef: not null, milkIntervalDays: > 0 })
        {
            Verse.ThingDef milkDef = milkableCompProps.milkDef;
            float milkNutrition = milkDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float milkAmount = milkableCompProps.milkAmount;
            float milkIntervalDays = milkableCompProps.milkIntervalDays;
            float milkPerDay = milkAmount / milkIntervalDays;

            milkNutritionPerDay = milkPerDay * milkNutrition;
        }

        // Eggs
        float eggsNutritionPerDay = 0f;
        CompProperties_EggLayer? eggLayerCompProps = record.EggLayerCompProperties;

        if (eggLayerCompProps is { eggLayIntervalDays: > 0 })
        {
            Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();
            float eggNutrition = eggDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float averageEggCount = eggLayerCompProps.eggCountRange.Average;
            float eggLayIntervalDays = eggLayerCompProps.eggLayIntervalDays;
            float eggsPerDay = averageEggCount / eggLayIntervalDays;

            eggsNutritionPerDay = eggsPerDay * eggNutrition;
        }

        // Result
        float productsNutritionPerDay = milkNutritionPerDay + eggsNutritionPerDay;

        return new NumberColumnCell(productsNutritionPerDay.ToDecimal(2), "0.00/d");
    }
}
