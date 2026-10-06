using System.Collections.Generic;
using System.Linq;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.ApparelDef;

public sealed class LayersColumn<TRecord> : DefSetColumn<TRecord> where TRecord : IThingDefTableRecord
{
    private static readonly List<ApparelLayerDef> _emptyList = [];

    public LayersColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetLayerDefs(thingDefs))
    {
    }

    public override bool AutoRefresh => false;

    protected override IReadOnlyCollection<Verse.Def>? GetDefs(TRecord record)
    {
        return record.ThingDef.apparel?.layers;
    }

    private static IEnumerable<Verse.Def> GetLayerDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .SelectMany(thingDef => thingDef.apparel?.layers ?? _emptyList)
            .Distinct();
    }
}
