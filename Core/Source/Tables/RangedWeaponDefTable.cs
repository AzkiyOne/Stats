using System.Collections.Generic;
using RimWorld;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class RangedWeaponDefTable : Tab
{
    private static readonly List<ThingDefTableRecord> _records;
    private static readonly List<ThingDef> _thingDefs;

    static RangedWeaponDefTable()
    {
        ThingDefTableUtils.GetThingDefRecordsStuffed(
            thingDef =>
                thingDef is { IsRangedWeapon: true, destroyOnDrop: false }
                && thingDef.GetCompProperties<CompProperties_UniqueWeapon>() == null,
            out List<ThingDefTableRecord> records,
            out List<ThingDef> thingDefs);

        _records = records;
        _thingDefs = thingDefs;
    }

    public RangedWeaponDefTable(TableDef def) : base(def)
    {
        Widget = new Table<ThingDefTableRecord>(def, _records, [_thingDefs]);
    }

    protected override TabBodyWidget Widget { get; }
}
