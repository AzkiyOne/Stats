using System.Collections.Generic;
using System.Linq;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.ApparelDef;

// In the game, this property is actually displayed as a list of all of the
// individual body parts that an apprel is covering. The resulting list may be
// huge. Displaying it in a single row will be a bad UX.
//
// Luckily, it looks like in a definition it is allowed to only list the whole
// groups of body parts. The resulting list is of course significantly smaller
// and can be safely displayed in a single row/column.
public sealed class BodyPartGroupsColumn<TRecord> : DefSetColumn<TRecord> where TRecord : IThingDefTableRecord
{
    private static readonly List<BodyPartGroupDef> _emptyList = [];

    public BodyPartGroupsColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetBodyPartGroupDefs(thingDefs))
    {
    }

    public override bool IsRefreshable => false;

    protected override IReadOnlyCollection<Verse.Def>? GetDefs(TRecord record)
    {
        return record.ThingDef.apparel?.bodyPartGroups;
    }

    private static IEnumerable<Verse.Def> GetBodyPartGroupDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .SelectMany(thingDef => thingDef.apparel?.bodyPartGroups ?? _emptyList)
            .Distinct();
    }
}
