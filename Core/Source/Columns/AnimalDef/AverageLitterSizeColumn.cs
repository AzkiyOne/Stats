using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class AverageLitterSizeColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public AverageLitterSizeColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;

        if (thingDef.race != null)
        {
            float averageLitterSize = AnimalProductionUtility.OffspringRange(thingDef).Average;

            return averageLitterSize.ToDecimal(1);
        }

        return 0m;
    }
}
