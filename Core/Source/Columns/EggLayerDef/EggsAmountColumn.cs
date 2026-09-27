using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.EggLayerDef;

public sealed class EggsAmountColumn<TRecord> : ThingDefCountColumn<TRecord> where TRecord : IEggLayerDefTableRecord
{
    public EggsAmountColumn(ColumnDef def) : base(def)
    {
    }

    protected override ThingDefCountColumnCell MakeCell(TRecord record)
    {
        CompProperties_EggLayer? eggLayerCompProps = record.EggLayerCompProperties;

        if (eggLayerCompProps != null)
        {
            Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();
            float eggAmount = eggLayerCompProps.eggCountRange.Average;

            return new ThingDefCountColumnCell(eggDef, eggAmount);
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_EggLayer>()?.GetAnyEggDef())
            .Distinct();
    }
}
