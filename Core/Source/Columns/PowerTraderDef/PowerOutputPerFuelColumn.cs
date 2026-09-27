using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PowerTraderDef;

public sealed class PowerOutputPerFuelColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public PowerOutputPerFuelColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0 W/u")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_Power? powerCompProps = record.ThingDef.GetCompProperties<CompProperties_Power>();

        if (powerCompProps != null)
        {
            CompProperties_Refuelable? refuelableCompProps = record.ThingDef.GetCompProperties<CompProperties_Refuelable>();

            if (refuelableCompProps is { fuelConsumptionRate: not 0f })
            {
                float powerOutput = powerCompProps.PowerConsumption * -1f;
                float fuelConsumptionRate = refuelableCompProps.fuelConsumptionRate;
                float powerOutputPerFuel = powerOutput / fuelConsumptionRate;

                return powerOutputPerFuel.ToDecimal();
            }
        }

        return 0m;
    }
}
