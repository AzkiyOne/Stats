using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.AnimalDef;

public sealed class GrowthTimeColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public GrowthTimeColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0 d")
    {
    }

    protected override decimal GetValueFromRecord(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;

        if (thingDef.race != null)
        {
            float growthTime = AnimalProductionUtility.DaysToAdulthood(thingDef);

            return growthTime.ToDecimal();
        }

        return 0m;
    }
}
