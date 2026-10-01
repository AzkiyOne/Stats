using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
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
        List<ThingDefTableRecord> records = new(200);
        List<ThingDef> thingDefs = new(100);

        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef is { IsRangedWeapon: true, destroyOnDrop: false }
                && thingDef.GetCompProperties<CompProperties_UniqueWeapon>() == null)
            {
                thingDefs.Add(thingDef);

                HashSet<ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (ThingDef stuffDef in stuffDefs)
                    {
                        ThingDefTableRecord record = new(thingDef, stuffDef);

                        records.Add(record);
                    }
                }
                else
                {
                    ThingDefTableRecord record = new(thingDef);

                    records.Add(record);
                }
            }
        }

        _records = records;
        _thingDefs = thingDefs;
    }

    public RangedWeaponDefTable(TableDef def) : base(def)
    {
        Widget = new Table<ThingDefTableRecord>(def, _records, [_thingDefs]);
    }

    protected override TabBodyWidget Widget { get; }
}
