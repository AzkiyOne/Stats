using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class LifeSpanColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public LifeSpanColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0 d")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        PlantProperties? plantProps = record.ThingDef.plant;

        if (plantProps?.LifespanDays > 0f)
        {
            return plantProps.LifespanDays.ToDecimal(1);
        }

        return 0m;
    }
}
