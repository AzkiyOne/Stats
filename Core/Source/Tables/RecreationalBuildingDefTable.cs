using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class RecreationalBuildingDefTable : Tab
{
    private static readonly List<ThingDefTableRecord> _records;
    private static readonly List<ThingDef> _thingDefs;

    static RecreationalBuildingDefTable()
    {
        ThingDefTableUtils.GetThingDefRecordsStuffed(
            thingDef =>
                thingDef.IsBuildingObtainableByPlayer()
                && thingDef.statBases?.GetStatValueFromList(StatDefOf.JoyGainFactor, 0f) > 0f,
            out List<ThingDefTableRecord> records,
            out List<ThingDef> thingDefs);

        _records = records;
        _thingDefs = thingDefs;
    }

    public RecreationalBuildingDefTable(TableDef def) : base(def)
    {
        Widget = new Table<ThingDefTableRecord>(def, _records, [_thingDefs]);
    }

    protected override TabBodyWidget Widget { get; }
}
