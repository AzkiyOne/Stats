using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class NutritionPerHarvestPerDayColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public NutritionPerHarvestPerDayColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.000/d")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        PlantProperties? plantProps = record.ThingDef.plant;

        if (plantProps is { harvestedThingDef: not null, growDays: > 0f })
        {
            float productNutrition = plantProps.harvestedThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float nutritionPerHarvest = plantProps.harvestYield * productNutrition;

            return (nutritionPerHarvest / plantProps.GetGrowDaysActual()).ToDecimal(3);
        }

        return 0m;
    }
}
