using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.MilkableDef;

public sealed class MilkAmountColumn<TRecord> : ThingDefCountColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public MilkAmountColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetMilkDefs(thingDefs))
    {
    }

    public override bool IsRefreshable => false;

    protected override ThingDefCount? GetThingDefCount(TRecord record)
    {
        CompProperties_Milkable? milkableCompProps = record.ThingDef.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            Verse.ThingDef milkDef = milkableCompProps.milkDef;
            int milkAmount = milkableCompProps.milkAmount;

            return new ThingDefCount(milkDef, milkAmount);
        }

        return null;
    }

    private static IEnumerable<Verse.ThingDef?> GetMilkDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Milkable>()?.milkDef)
            .Distinct();
    }
}
