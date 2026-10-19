using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.AnimalDef;

public sealed class LeatherPerDayColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public LeatherPerDayColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0/d")
    {
    }

    protected override decimal GetValueFromRecord(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        float growthTime = AnimalProductionUtility.DaysToAdulthood(thingDef);

        if (growthTime > 0f)
        {
            float leatherAmount = thingDef.GetStatValuePerceived(StatDefOf.LeatherAmount);
            float leatherPerDay = leatherAmount / growthTime;

            return leatherPerDay.ToDecimal(1);
        }

        return 0m;
    }
}
