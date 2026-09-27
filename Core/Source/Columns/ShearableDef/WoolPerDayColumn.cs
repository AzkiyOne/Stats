using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.ShearableDef;

public sealed class WoolPerDayColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public WoolPerDayColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0/d")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_Shearable? shearableCompProps = record.ThingDef.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps is { shearIntervalDays: > 0 })
        {
            float woolAmount = shearableCompProps.woolAmount;
            float shearIntervalDays = shearableCompProps.shearIntervalDays;
            float woolPerDay = woolAmount / shearIntervalDays;

            return woolPerDay.ToDecimal(1);
        }

        return 0m;
    }
}
