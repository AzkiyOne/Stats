using HarmonyLib;
using RimWorld;
using Verse;

namespace Stats;

[StaticConstructorOnStartup]
public static class HarmonyPatches
{
    static HarmonyPatches()
    {
        Harmony harmony = new("Azzkiy.Stats");

        harmony.Patch(
            AccessTools.Method(typeof(MapEvents), nameof(MapEvents.Notify_ThingSpawned)),
            postfix: new HarmonyMethod(typeof(Events), nameof(Events.NotifyThingSpawned))
        );
        harmony.Patch(
            AccessTools.Method(typeof(MapEvents), nameof(MapEvents.Notify_ThingDespawned)),
            postfix: new HarmonyMethod(typeof(Events), nameof(Events.NotifyThingDespawned))
        );
        harmony.Patch(
            AccessTools.Method(typeof(Prefs), nameof(Prefs.Save)),
            postfix: new HarmonyMethod(typeof(Events), nameof(Events.NotifyPrefsChanged))
        );
        harmony.Patch(
            AccessTools.Method(typeof(Page_SelectStorytellerInGame), nameof(Page_SelectStorytellerInGame.PreClose)),
            postfix: new HarmonyMethod(typeof(Events), nameof(Events.NotifyDifficultyChanged))
        );
    }
}
