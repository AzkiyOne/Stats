using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.EggLayerDef;

public sealed class EggsPerDayColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public EggsPerDayColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0/d")
    {
    }

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_EggLayer? eggLayerCompProps = record.ThingDef.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps is { eggLayIntervalDays: > 0f })
        {
            float eggLayIntervalDays = eggLayerCompProps.eggLayIntervalDays;
            float averageEggCount = eggLayerCompProps.eggCountRange.Average;
            float eggsPerDay = averageEggCount / eggLayIntervalDays;

            return eggsPerDay.ToDecimal(1);
        }

        return 0m;
    }
}
