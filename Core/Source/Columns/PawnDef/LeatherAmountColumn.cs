using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns.Cells;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Tables;

namespace Stats.Columns.PawnDef;

public sealed class LeatherAmountColumn<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumn<TRecord, ThingDefCountColumnCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override ThingDefCountColumnCell MakeCell(TRecord record)
    {
        Verse.ThingDef? leatherDef = record.RaceProperties.leatherDef;

        if (leatherDef != null)
        {
            Verse.ThingDef thingDef = record.ThingDef;
            float leatherAmount = thingDef.GetStatValuePerceived(StatDefOf.LeatherAmount);

            if (leatherAmount > 0f)
            {
                return new ThingDefCountColumnCell(leatherDef, leatherAmount);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.race?.leatherDef)
            .Distinct();
    }
}
