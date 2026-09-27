using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class RangedWeaponDefTable : Tab
{
    public RangedWeaponDefTable(TableDef def) : base(def)
    {
        Widget = new Table<ThingDefTableRecord>(def, Records, [Records.Select(rec => rec.ThingDef)]);
    }

    protected override TabBodyWidget Widget { get; }

    static RangedWeaponDefTable()
    {
        List<ThingDefTableRecord> records = new(100);

        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef is { IsRangedWeapon: true, destroyOnDrop: false }
                && thingDef.GetCompProperties<CompProperties_UniqueWeapon>() == null)
            {
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

        Records = records;
    }

    private static List<ThingDefTableRecord> Records { get; }
}
