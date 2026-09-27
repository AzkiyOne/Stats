using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.RefuelableDef;

public sealed class DaysPerRefuelColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public DaysPerRefuelColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0 d")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_Refuelable? refuelableCompProps = record.ThingDef.GetCompProperties<CompProperties_Refuelable>();

        if (refuelableCompProps is { fuelConsumptionRate: not 0f })
        {
            float fuelCapacity = refuelableCompProps.fuelCapacity;
            float fuelConsumptionRate = refuelableCompProps.fuelConsumptionRate;
            float daysPerRefuel = fuelCapacity / fuelConsumptionRate;

            return daysPerRefuel.ToDecimal(1);
        }

        return 0m;
    }
}
