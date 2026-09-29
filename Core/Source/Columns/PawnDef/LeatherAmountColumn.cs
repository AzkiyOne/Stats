using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.PawnDef;

public sealed class LeatherAmountColumn<TRecord> : ThingDefCountColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public LeatherAmountColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetLeatherDefs(thingDefs))
    {
    }

    public override bool IsRefreshable => false;

    protected override ThingDefCount? GetValueFromRecord(TRecord record)
    {
        Verse.ThingDef? leatherDef = record.ThingDef.race?.leatherDef;

        if (leatherDef != null)
        {
            Verse.ThingDef thingDef = record.ThingDef;
            float leatherAmount = thingDef.GetStatValuePerceived(StatDefOf.LeatherAmount);

            if (leatherAmount > 0f)
            {
                return new ThingDefCount(leatherDef, (int)leatherAmount);
            }
        }

        return null;
    }

    private static IEnumerable<Verse.ThingDef?> GetLeatherDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .Select(thingDef => thingDef.race?.leatherDef)
            .Distinct();
    }
}
