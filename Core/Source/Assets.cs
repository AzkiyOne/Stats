using UnityEngine;
using Verse;

namespace Stats;

[StaticConstructorOnStartup]
public static class Assets
{
    static Assets()
    {
        TableFiltersTabIcon = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/Filter");
        TableColumnsMenuIcon = ContentFinder<Texture2D>.Get("UI/Buttons/OpenSpecificTab");
    }

    internal static Texture2D TableFiltersTabIcon { get; }

    internal static Texture2D TableColumnsMenuIcon { get; }
}
