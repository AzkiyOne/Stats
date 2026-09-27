using System.Collections.Generic;
using System.Linq;
using Stats.Columns.Cells;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Tables;

namespace Stats.Columns.PawnDef;

public sealed class WeaponsColumn<TRecord>(ColumnDef columnDef) :
    ThingDefSetColumn<TRecord, ThingDefSetColumnCell>(columnDef)
        where TRecord :
            IThingDefTableRecord
{
    protected override ThingDefSetColumnCell MakeCell(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        HashSet<Verse.ThingDef>? weapons = thingDef.GetPossibleWeapons();

        if (weapons != null)
        {
            return new ThingDefSetColumnCell(weapons);
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetValueFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .SelectMany(thingDef => thingDef.GetPossibleWeapons() ?? [])
            .Distinct();
    }
}
