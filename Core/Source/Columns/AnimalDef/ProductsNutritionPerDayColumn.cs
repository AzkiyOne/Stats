using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class ProductsNutritionPerDayColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public ProductsNutritionPerDayColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.00/d")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        // Milk
        float milkNutritionPerDay = 0f;
        CompProperties_Milkable? milkableCompProps = record.ThingDef.GetCompProperties<CompProperties_Milkable>();

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
        CompProperties_EggLayer? eggLayerCompProps = record.ThingDef.GetCompProperties<CompProperties_EggLayer>();

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

        return productsNutritionPerDay.ToDecimal(2);
    }
}
