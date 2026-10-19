using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.RefuelableDef;

public sealed class FuelConsumptionRateColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public FuelConsumptionRateColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0/d")
    {
    }

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_Refuelable? refuelableCompProps = record.ThingDef.GetCompProperties<CompProperties_Refuelable>();

        if (refuelableCompProps != null)
        {
            // TODO: Difficulty scaling.
            return refuelableCompProps.fuelConsumptionRate.ToDecimal(1);
        }

        return 0m;
    }
}
