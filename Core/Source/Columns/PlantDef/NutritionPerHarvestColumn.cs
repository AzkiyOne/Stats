using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class NutritionPerHarvestColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public NutritionPerHarvestColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.00")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        PlantProperties? plantProps = record.ThingDef.plant;

        if (plantProps?.harvestedThingDef != null)
        {
            float productNutrition = plantProps.harvestedThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float nutritionPerHarvest = plantProps.harvestYield * productNutrition;

            return nutritionPerHarvest.ToDecimal(2);
        }

        return 0m;
    }
}
