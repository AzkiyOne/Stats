using System.Collections.Generic;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class PlantDefTable : Tab
{
    private static readonly List<ThingDefTableRecord> _records;
    private static readonly List<ThingDef> _thingDefs;

    static PlantDefTable()
    {
        ThingDefTableUtils.GetThingDefRecords(
            thingDef => thingDef is { IsPlant: true, plant.isStump: false },
            out List<ThingDefTableRecord> records,
            out List<ThingDef> thingDefs);

        _records = records;
        _thingDefs = thingDefs;
    }

    public PlantDefTable(TableDef def) : base(def)
    {
        Widget = new Table<ThingDefTableRecord>(def, _records, [_thingDefs]);
    }

    protected override TabBodyWidget Widget { get; }
}
