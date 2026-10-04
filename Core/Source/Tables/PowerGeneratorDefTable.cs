using System.Collections.Generic;
using RimWorld;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class PowerGeneratorDefTable : Tab
{
    private static readonly List<ThingDefTableRecord> _records;
    private static readonly List<ThingDef> _thingDefs;

    static PowerGeneratorDefTable()
    {
        ThingDefTableUtils.GetThingDefRecordsStuffed(
            thingDef => thingDef.GetCompProperties<CompProperties_Power>()?.PowerConsumption < 0f,
            out List<ThingDefTableRecord> records,
            out List<ThingDef> thingDefs);

        _records = records;
        _thingDefs = thingDefs;
    }

    public PowerGeneratorDefTable(TableDef def) : base(def)
    {
        Widget = new Table<ThingDefTableRecord>(def, _records, [_thingDefs]);
    }

    protected override TabBodyWidget Widget { get; }
}
