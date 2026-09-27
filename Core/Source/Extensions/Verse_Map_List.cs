using System.Collections.Generic;
using Verse;

namespace Stats.Extensions;

public static class Verse_Map_List
{
    public static IEnumerable<Thing> GetSpawnedThings(this List<Map> maps)
    {
        foreach (Map map in maps)
        {
            foreach (Thing thing in map.spawnedThings)
            {
                yield return thing;
            }
        }
    }
}
