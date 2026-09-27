using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns.Cells;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Tables;
using Verse;

namespace Stats.Columns.PawnDef;

public sealed class MeatAmountColumn<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumn<TRecord, ThingDefCountColumnCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override ThingDefCountColumnCell MakeCell(TRecord record)
    {
        RaceProperties raceProperties = record.RaceProperties;
        Verse.ThingDef? meatDef = raceProperties.meatDef;

        if (meatDef != null)
        {
            Verse.ThingDef thingDef = record.ThingDef;
            float meatAmount = thingDef.GetStatValuePerceived(StatDefOf.MeatAmount);

            if (meatAmount > 0f)
            {
                return new ThingDefCountColumnCell(meatDef, meatAmount);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.race?.meatDef)
            .Distinct();
    }
}
