using System;
using RimWorld;
using Verse;

namespace Stats;

public static class Events
{
    internal static event Action? ResearchCompleted;
    public static event Action<Thing>? ThingSpawned;
    public static event Action<Thing>? ThingDespawned;
    // TODO:
    // - Implementation.
    // - It would better to track individual values.
    public static event Action? PrefsChanged;

    static Events()
    {
        Find.SignalManager.RegisterReceiver(new ResearchCompletedSignalReceiver());
    }

    internal static void NotifyThingSpawned(Thing thing)
    {
        ThingSpawned?.Invoke(thing);
    }

    internal static void NotifyThingDespawned(Thing thing)
    {
        ThingDespawned?.Invoke(thing);
    }

    private sealed class ResearchCompletedSignalReceiver : ISignalReceiver
    {
        public void Notify_SignalReceived(Signal signal)
        {
            if (signal.tag == "ResearchCompleted")
            {
                ResearchCompleted?.Invoke();
            }
        }
    }
}
