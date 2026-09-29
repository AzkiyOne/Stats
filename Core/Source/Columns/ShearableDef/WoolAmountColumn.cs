using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.ShearableDef;

public sealed class WoolAmountColumn<TRecord> : ThingDefCountColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public WoolAmountColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetWoolDefs(thingDefs))
    {
    }

    public override bool IsRefreshable => false;

    protected override ThingDefCount? GetValueFromRecord(TRecord record)
    {
        CompProperties_Shearable? shearableCompProps = record.ThingDef.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            Verse.ThingDef woolDef = shearableCompProps.woolDef;
            int woolAmount = shearableCompProps.woolAmount;

            return new ThingDefCount(woolDef, woolAmount);
        }

        return null;
    }

    private static IEnumerable<Verse.ThingDef?> GetWoolDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Shearable>()?.woolDef)
            .Distinct();
    }
}
