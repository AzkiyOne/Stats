using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stats.Extensions;

public static class RimWorld_StuffCategoryDef
{
    private static readonly Dictionary<StuffCategoryDef, HashSet<ThingDef>> _stuffsByCategory = [];

    static RimWorld_StuffCategoryDef()
    {
        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef.stuffProps == null) continue;

            foreach (StuffCategoryDef stuffCategoryDef in thingDef.stuffProps.categories)
            {
                bool exists = _stuffsByCategory.TryGetValue(stuffCategoryDef, out HashSet<ThingDef>? categoryStuffs);

                if (exists)
                {
                    categoryStuffs.Add(thingDef);
                }
                else
                {
                    _stuffsByCategory[stuffCategoryDef] = [thingDef];
                }
            }
        }
    }

    public static HashSet<ThingDef> GetStuffDefs(this StuffCategoryDef stuffCategoryDef)
    {
        return _stuffsByCategory[stuffCategoryDef];
    }
}
