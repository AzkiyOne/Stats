using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class TurretDefTable : Tab
{
    private static readonly List<ThingDefTableRecord> _records;
    private static readonly List<ThingDef> _thingDefs;

    static TurretDefTable()
    {
        ThingDefTableUtils.GetThingDefRecordsStuffed(
            thingDef =>
                thingDef is { building.IsTurret: true }
                && thingDef.IsBuildingObtainableByPlayer(),
            out List<ThingDefTableRecord> records,
            out List<ThingDef> thingDefs);

        _records = records;
        _thingDefs = thingDefs;
    }

    public TurretDefTable(TableDef def) : base(def)
    {
        Widget = new Table<ThingDefTableRecord>(def, _records, [_thingDefs]);
    }

    protected override TabBodyWidget Widget { get; }
}
