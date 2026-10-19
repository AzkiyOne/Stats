using RimWorld;
using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.ShearableDef;

public sealed class ShearingIntervalColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public ShearingIntervalColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0 d")
    {
    }

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_Shearable? shearableCompProps = record.ThingDef.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            return shearableCompProps.shearIntervalDays;
        }

        return 0m;
    }
}
