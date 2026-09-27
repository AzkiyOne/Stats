using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace Stats.Extensions;

public static class Verse_ThingDef
{
    private static readonly Dictionary<ThingDef, HashSet<RecipeDef>> _thingDefRecipes = [];
    private static readonly Dictionary<ThingDef, HashSet<PawnKindDef>> _pawnKinds = [];
    private static readonly Dictionary<string, HashSet<ThingDef>> _weaponsByTag = [];
    private static readonly Dictionary<ThingDef, HashSet<ResearchProjectDef>> _researchProjects = [];

    static Verse_ThingDef()
    {
        foreach (RecipeDef recipeDef in DefDatabase<RecipeDef>.AllDefsListForReading)
        {
            ThingDef producedThingDef = recipeDef.ProducedThingDef;

            if (producedThingDef != null)
            {
                bool recipesEntryExists = _thingDefRecipes.TryGetValue(producedThingDef, out HashSet<RecipeDef>? recipeDefs);

                if (recipesEntryExists)
                {
                    recipeDefs.Add(recipeDef);
                }
                else
                {
                    _thingDefRecipes[producedThingDef] = [recipeDef];
                }
            }
        }

        foreach (PawnKindDef pawnKindDef in DefDatabase<PawnKindDef>.AllDefsListForReading)
        {
            if (pawnKindDef.race == null)
            {
                continue;
            }

            _pawnKinds.TryGetValue(pawnKindDef.race, out HashSet<PawnKindDef>? pawnKindDefs);

            if (pawnKindDefs != null)
            {
                pawnKindDefs.Add(pawnKindDef);
            }
            else
            {
                _pawnKinds[pawnKindDef.race] = [pawnKindDef];
            }
        }

        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef.IsWeapon && thingDef.weaponTags?.Count > 0)
            {
                foreach (string tag in thingDef.weaponTags)
                {
                    _weaponsByTag.TryGetValue(tag, out HashSet<ThingDef>? weapons);

                    if (weapons != null)
                    {
                        weapons.Add(thingDef);
                    }
                    else
                    {
                        _weaponsByTag[tag] = [thingDef];
                    }
                }
            }
        }

        foreach (ResearchProjectDef researchProjectDef in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
        {
            foreach (Def def in researchProjectDef.UnlockedDefs)
            {
                if (def is ThingDef thingDef)
                {
                    _researchProjects.TryGetValue(thingDef, out HashSet<ResearchProjectDef>? researchProjects);

                    if (researchProjects != null)
                    {
                        researchProjects.Add(researchProjectDef);
                    }
                    else
                    {
                        _researchProjects[thingDef] = [researchProjectDef];
                    }
                }
            }
        }
    }

    // GenStuff.DefaultStuffFor() is a bit too heavy for some tasks.
    private static readonly Func<ThingDef, ThingDef?> GetDefaultStuffCached =
    System_Function.Memoized((ThingDef thingDef) =>
    {
        return GenStuff.DefaultStuffFor(thingDef);
    });

    // Verse.ThingDef.defaultStuff isn't actually what is advertised.
    public static ThingDef? GetDefaultStuff(this ThingDef thingDef)
    {
        return GetDefaultStuffCached(thingDef);
    }

    public static bool IsBuildingObtainableByPlayer(this ThingDef thingDef)
    {
        return thingDef.BuildableByPlayer || thingDef.Minifiable;
    }

    public static HashSet<RecipeDef>? GetRecipeDefs(this ThingDef thingDef)
    {
        _thingDefRecipes.TryGetValue(thingDef, out HashSet<RecipeDef>? recipeDefs);

        return recipeDefs;
    }

    public static float GetStatValuePerceived(this ThingDef thingDef, StatDef statDef, ThingDef? stuffDef = null)
    {
        StatRequest statRequest = StatRequest.For(thingDef, stuffDef);

        if (statDef.Worker.ShouldShowFor(statRequest))
        {
            return statDef.Worker.GetValue(statRequest);
        }

        return 0f;
    }

    public static HashSet<PawnKindDef>? GetPawnKindDefs(this ThingDef thingDef)
    {
        _pawnKinds.TryGetValue(thingDef, out HashSet<PawnKindDef>? pawnKindDefs);

        return pawnKindDefs;
    }

    // TODO: Cache this too?
    public static HashSet<ThingDef>? GetPossibleWeapons(this ThingDef thingDef)
    {
        HashSet<PawnKindDef>? pawnKindDefs = thingDef.GetPawnKindDefs();

        if (pawnKindDefs?.Count > 0)
        {
            HashSet<ThingDef> result = [];

            foreach (PawnKindDef pawnKindDef in pawnKindDefs)
            {
                if (pawnKindDef.weaponTags != null)
                {
                    foreach (string tag in pawnKindDef.weaponTags)
                    {
                        _weaponsByTag.TryGetValue(tag, out HashSet<ThingDef>? weapons);

                        if (weapons != null)
                        {
                            result.AddRange(weapons);
                        }
                    }
                }
            }

            return result;
        }

        return null;
    }

    public static HashSet<ResearchProjectDef>? GetResearchProjectDefs(this ThingDef thingDef)
    {
        _researchProjects.TryGetValue(thingDef, out HashSet<ResearchProjectDef>? researchProjectDefs);

        return researchProjectDefs;
    }

    public static HashSet<ThingDef>? GetAllowedStuffs(this ThingDef thingDef)
    {
        if (thingDef.stuffCategories?.Count > 0)
        {
            HashSet<ThingDef> result = new(30);

            foreach (StuffCategoryDef stuffCategoryDef in thingDef.stuffCategories)
            {
                result.AddRange(stuffCategoryDef.GetStuffDefs());
            }

            return result;
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThingDef GetTurretGunDefOrSelf(this ThingDef thingDef)
    {
        return thingDef.building?.turretGunDef ?? thingDef;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VerbProperties? GetGunPrimaryVerbProps(this ThingDef thingDef)
    {
        return thingDef.GetTurretGunDefOrSelf().Verbs.Primary();
    }
}
