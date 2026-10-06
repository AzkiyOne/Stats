using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class ProductPerDayColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public ProductPerDayColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.00/d")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        PlantProperties? plantProps = record.ThingDef.plant;

        if (plantProps?.growDays > 0f)
        {
            return (plantProps.harvestYield / plantProps.GetGrowDaysActual()).ToDecimal(2);
        }

        return 0m;
    }
}
