using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class GestationTimeColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public GestationTimeColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0 d")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;

        if (thingDef.race != null)
        {
            float gestationTime = AnimalProductionUtility.GestationDaysLitter(thingDef);

            return gestationTime.ToDecimal(1);
        }

        return 0m;
    }
}
