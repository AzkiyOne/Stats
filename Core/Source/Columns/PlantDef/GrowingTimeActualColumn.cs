using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class GrowingTimeActualColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public GrowingTimeActualColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0 d")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        PlantProperties? plantProps = record.ThingDef.plant;

        if (plantProps?.growDays > 0f)
        {
            return plantProps.GetGrowDaysActual().ToDecimal(1);
        }

        return 0m;
    }
}
