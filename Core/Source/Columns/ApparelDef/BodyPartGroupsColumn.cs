using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Tables;

namespace Stats.Columns.ApparelDef;

// In the game, this property is actually displayed as a list of all of the
// individual body parts that an apprel is covering. The resulting list may be
// huge. Displaying it in a single row will be a bad UX.
//
// Luckily, it looks like in a definition it is allowed to only list the whole
// groups of body parts. The resulting list is of course significantly smaller
// and can be safely displayed in a single row/column.
public sealed class BodyPartGroupsColumn<TRecord>(ColumnDef columnDef) :
    DefSetColumn<TRecord, DefSetColumnCell>(columnDef)
        where TRecord :
            IApparelDefTableRecord
{
    protected override DefSetColumnCell MakeCell(TRecord record)
    {
        ApparelProperties apparelProps = record.ApparelProperties;
        List<Verse.BodyPartGroupDef> bodyPartGroups = apparelProps.bodyPartGroups;

        return new DefSetColumnCell(bodyPartGroups);
    }

    protected override IEnumerable<Verse.Def?> GetValueFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .SelectMany(thingDef => thingDef.apparel?.bodyPartGroups)
            .Distinct();
    }
}
