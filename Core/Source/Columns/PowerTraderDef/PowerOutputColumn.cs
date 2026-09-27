using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PowerTraderDef;

public sealed class PowerOutputColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public PowerOutputColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0 W")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_Power? powerCompProps = record.ThingDef.GetCompProperties<CompProperties_Power>();

        if (powerCompProps is { PowerConsumption: < 0f })
        {
            float powerOutput = powerCompProps.PowerConsumption * -1f;

            return powerOutput.ToDecimal();
        }

        return 0m;
    }
}
