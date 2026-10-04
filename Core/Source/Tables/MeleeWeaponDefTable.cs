using System.Collections.Generic;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class MeleeWeaponDefTable : Tab
{
    private static readonly List<ThingDefTableRecord> _records;
    private static readonly List<ThingDef> _thingDefs;

    static MeleeWeaponDefTable()
    {
        ThingDefTableUtils.GetThingDefRecordsStuffed(
            thingDef => thingDef is { IsMeleeWeapon: true, destroyOnDrop: false },
            out List<ThingDefTableRecord> records,
            out List<ThingDef> thingDefs);

        _records = records;
        _thingDefs = thingDefs;
    }

    public MeleeWeaponDefTable(TableDef def) : base(def)
    {

        Widget = new Table<ThingDefTableRecord>(def, _records, [_thingDefs]);
    }

    protected override TabBodyWidget Widget { get; }
}
