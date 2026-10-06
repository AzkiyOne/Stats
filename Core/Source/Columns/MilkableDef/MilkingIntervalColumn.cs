using System.Collections.Generic;
using RimWorld;
using Stats.TableRecords;

namespace Stats.Columns.MilkableDef;

public sealed class MilkingIntervalColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public MilkingIntervalColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0 d")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_Milkable? milkableCompProps = record.ThingDef.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            return milkableCompProps.milkIntervalDays;
        }

        return 0m;
    }
}
