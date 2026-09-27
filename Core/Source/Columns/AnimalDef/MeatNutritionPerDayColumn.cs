using RimWorld;
using Stats.Columns.Cells;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.AnimalDef;

public sealed class MeatNutritionPerDayColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberColumnCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberColumnCell MakeCell(TRecord record)
    {
        RaceProperties raceProps = record.RaceProperties;
        Verse.ThingDef? meatDef = raceProps.meatDef;

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

                return new NumberColumnCell(meatNutritionPerDay.ToDecimal(2), "0.00/d");
            }
        }

        return default;
    }
}
