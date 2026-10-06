using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class GrowthTimeColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public GrowthTimeColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0 d")
    {
    }

    public override bool AutoRefresh => false;

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
