using System;
using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Tables;

public static class ThingDefTableUtils
{
    public static void GetThingDefRecords(Func<ThingDef, bool> predicate, out List<ThingDefTableRecord> records, out List<ThingDef> thingDefs)
    {
        records = new(250);
        thingDefs = new(100);

        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (predicate(thingDef))
            {
                ThingDefTableRecord record = new(thingDef);

                records.Add(record);
                thingDefs.Add(thingDef);
            }
        }
    }

    public static void GetThingDefRecordsStuffed(Func<ThingDef, bool> predicate, out List<ThingDefTableRecord> records, out List<ThingDef> thingDefs)
    {
        records = new(250);
        thingDefs = new(100);

        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (predicate(thingDef))
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
    }
}
