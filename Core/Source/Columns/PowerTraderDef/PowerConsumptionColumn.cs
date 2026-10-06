using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PowerTraderDef;

public sealed class PowerConsumptionColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public PowerConsumptionColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0 W")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_Power? powerCompProps = record.ThingDef.GetCompProperties<CompProperties_Power>();

        if (powerCompProps is { PowerConsumption: > 0f })
        {
            float powerConsumption = powerCompProps.PowerConsumption;

            return powerConsumption.ToDecimal();
        }

        return 0m;
    }
}
