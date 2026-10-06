using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class RawNutritionPerDayColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public RawNutritionPerDayColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.000/d")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        PlantProperties? plantProps = record.ThingDef.plant;

        if (plantProps?.growDays > 0f)
        {
            float nutrition = record.ThingDef.GetStatValuePerceived(StatDefOf.Nutrition);

            return (nutrition / plantProps.GetGrowDaysActual()).ToDecimal(3);
        }

        return 0m;
    }
}
