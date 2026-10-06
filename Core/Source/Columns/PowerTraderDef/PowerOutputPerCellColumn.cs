using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PowerTraderDef;

public sealed class PowerOutputPerCellColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public PowerOutputPerCellColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0 W/c")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_Power? powerCompProps = record.ThingDef.GetCompProperties<CompProperties_Power>();

        if (powerCompProps != null)
        {
            float area = record.ThingDef.size.Area;
            float powerOutput = powerCompProps.PowerConsumption * -1f;
            float powerOutputPerCell = powerOutput / area;

            return powerOutputPerCell.ToDecimal();
        }

        return 0m;
    }
}
