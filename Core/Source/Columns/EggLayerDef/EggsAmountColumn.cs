using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.EggLayerDef;

public sealed class EggsAmountColumn<TRecord> : ThingDefCountColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public EggsAmountColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetEggDefs(thingDefs))
    {
    }

    public override bool IsRefreshable => false;

    protected override ThingDefCount? GetThingDefCount(TRecord record)
    {
        CompProperties_EggLayer? eggLayerCompProps = record.ThingDef.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps != null)
        {
            Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();
            float eggAmount = eggLayerCompProps.eggCountRange.Average;

            return new ThingDefCount(eggDef, (int)eggAmount);
        }

        return null;
    }

    private static IEnumerable<Verse.ThingDef?> GetEggDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_EggLayer>()?.GetAnyEggDef())
            .Distinct();
    }
}
