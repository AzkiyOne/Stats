using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class ArtBuildingDefTable : Tab
{
    private static readonly List<ThingDefTableRecord> _records;
    private static readonly List<ThingDef> _thingDefs;

    static ArtBuildingDefTable()
    {
        ThingDefTableUtils.GetThingDefRecordsStuffed(
            thingDef => thingDef.building != null && thingDef.IsBuildingObtainableByPlayer() && thingDef.IsArt,
            out List<ThingDefTableRecord> records,
            out List<ThingDef> thingDefs);

        _records = records;
        _thingDefs = thingDefs;
    }

    public ArtBuildingDefTable(TableDef def) : base(def)
    {
        Widget = new Table<ThingDefTableRecord>(def, _records, [_thingDefs]);
    }

    protected override TabBodyWidget Widget { get; }
}
