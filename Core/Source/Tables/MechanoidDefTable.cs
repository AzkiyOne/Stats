using System.Collections.Generic;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class MechanoidDefTable : Tab
{
    private static readonly List<ThingDefTableRecord> _records;
    private static readonly List<ThingDef> _thingDefs;

    static MechanoidDefTable()
    {
        ThingDefTableUtils.GetThingDefRecords(
            thingDef => thingDef is { race.IsMechanoid: true, IsCorpse: false },
            out List<ThingDefTableRecord> records,
            out List<ThingDef> thingDefs);

        _records = records;
        _thingDefs = thingDefs;
    }

    public MechanoidDefTable(TableDef def) : base(def)
    {
        Widget = new Table<ThingDefTableRecord>(def, _records, [_thingDefs]);
    }

    protected override TabBodyWidget Widget { get; }
}
