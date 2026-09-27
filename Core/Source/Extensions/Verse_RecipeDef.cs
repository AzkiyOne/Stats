using System.Collections.Generic;
using Verse;

namespace Stats.Extensions;

public static class Verse_RecipeDef
{
    private static readonly Dictionary<RecipeDef, HashSet<ThingDef>> _recipeUsers = [];

    static Verse_RecipeDef()
    {
        foreach (RecipeDef recipeDef in DefDatabase<RecipeDef>.AllDefsListForReading)
        {
            if (recipeDef.recipeUsers?.Count > 0)
            {
                bool recipeUsersEntryExists = _recipeUsers.TryGetValue(recipeDef, out HashSet<ThingDef>? recipeUsers);

                if (recipeUsersEntryExists)
                {
                    recipeUsers.AddRange(recipeDef.recipeUsers);
                }
                else
                {
                    _recipeUsers[recipeDef] = [.. recipeDef.recipeUsers];
                }
            }
        }

        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef.recipes?.Count > 0)
            {
                foreach (RecipeDef? recipeDef in thingDef.recipes)
                {
                    bool recipeUsersEntryExists = _recipeUsers.TryGetValue(recipeDef, out HashSet<ThingDef>? recipeUsers);

                    if (recipeUsersEntryExists)
                    {
                        recipeUsers.Add(thingDef);
                    }
                    else
                    {
                        _recipeUsers[recipeDef] = [thingDef];
                    }
                }
            }
        }
    }

    public static HashSet<ThingDef>? GetAllRecipeUsers(this RecipeDef recipeDef)
    {
        _recipeUsers.TryGetValue(recipeDef, out HashSet<ThingDef>? recipeUsers);

        return recipeUsers;
    }
}
