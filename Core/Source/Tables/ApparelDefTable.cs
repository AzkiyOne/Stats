using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

// We do not check for "destroyOnDrop" for better compatibility with mods like
// VFE - Pirates.
public sealed class ApparelDefTable : Tab
{
    public ApparelDefTable(TableDef def) : base(def)
    {
        List<ApparelDefTableRecord> records = new(250);

        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            ApparelProperties? apparelProperties = thingDef.apparel;

            if (apparelProperties != null)
            {
                HashSet<Verse.ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (Verse.ThingDef stuffDef in stuffDefs)
                    {
                        ApparelDefTableRecord record = new(thingDef, apparelProperties, stuffDef);
                        records.Add(record);
                    }
                }
                else
                {
                    ApparelDefTableRecord record = new(thingDef, apparelProperties);
                    records.Add(record);
                }
            }
        }

        Widget = new Table<ApparelDefTableRecord>(def, records);
    }

    protected override TabBodyWidget Widget { get; }
}
