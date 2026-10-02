using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.PawnDef;

public sealed class MeatAmountColumn<TRecord> : ThingDefCountColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public MeatAmountColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetMeatDefs(thingDefs))
    {
    }

    public override bool IsRefreshable => false;

    protected override ThingDefCount? GetThingDefCount(TRecord record)
    {
        Verse.ThingDef? meatDef = record.ThingDef.race?.meatDef;

        if (meatDef != null)
        {
            Verse.ThingDef thingDef = record.ThingDef;
            float meatAmount = thingDef.GetStatValuePerceived(StatDefOf.MeatAmount);

            if (meatAmount > 0f)
            {
                return new ThingDefCount(meatDef, (int)meatAmount);
            }
        }

        return null;
    }

    private static IEnumerable<Verse.ThingDef?> GetMeatDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .Select(thingDef => thingDef.race?.meatDef)
            .Distinct();
    }
}
