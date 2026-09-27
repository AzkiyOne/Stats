using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class MeleeWeaponDefTable : Tab
{
    public MeleeWeaponDefTable(TableDef def) : base(def)
    {
        List<ThingDefTableRecord> records = new(250);

        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            if (thingDef is { IsMeleeWeapon: true, destroyOnDrop: false })
            {
                HashSet<Verse.ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (Verse.ThingDef stuffDef in stuffDefs)
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

        Widget = new Table<ThingDefTableRecord>(def, records);
    }

    protected override TabBodyWidget Widget { get; }
}
