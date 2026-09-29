using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class MeatNutritionPerDayColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public MeatNutritionPerDayColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.00/d")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        Verse.ThingDef? meatDef = record.ThingDef.race?.meatDef;

        if (meatDef != null)
        {
            Verse.ThingDef thingDef = record.ThingDef;
            float growthTime = AnimalProductionUtility.DaysToAdulthood(thingDef);

            if (growthTime > 0f)
            {
                float meatNutrition = meatDef.GetStatValuePerceived(StatDefOf.Nutrition);
                float meatAmount = AnimalProductionUtility.AdultMeatAmount(thingDef);
                float meatPerDay = meatAmount / growthTime;
                float meatNutritionPerDay = meatPerDay * meatNutrition;

                return meatNutritionPerDay.ToDecimal(2);
            }
        }

        return 0m;
    }
}
