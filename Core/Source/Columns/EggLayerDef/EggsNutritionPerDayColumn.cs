using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.EggLayerDef;

public sealed class EggsNutritionPerDayColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public EggsNutritionPerDayColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.00/d")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_EggLayer? eggLayerCompProps = record.ThingDef.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps is { eggLayIntervalDays: > 0f })
        {
            Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();
            float eggNutrition = eggDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float eggsPerDay = eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays;
            float eggsNutritionPerDay = eggsPerDay * eggNutrition;

            return eggsNutritionPerDay.ToDecimal(2);
        }

        return 0m;
    }
}
