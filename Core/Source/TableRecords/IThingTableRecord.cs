using RimWorld;
using Verse;

namespace Stats.TableRecords;

public interface IThingTableRecord : IThingDefTableRecord
{
    Thing Thing { get; }
}

public interface IThingWithCompsTableRecord : IThingTableRecord
{
    ThingWithComps ThingWithComps { get; }
}

public readonly record struct ThingTableRecord : IThingTableRecord
{
    public ThingTableRecord(Thing thing)
    {
        Thing = thing;
    }

    public Def Def => Thing.def;

    public BuildableDef BuildableDef => Thing.def;

    public ThingDef ThingDef => Thing.def;

    // Note: Things can change their stuff/quality (through dev tools/mods), so it'll be safer to recreate StatRequest each time.
    public StatRequest StatRequest => StatRequest.For(Thing);

    public Thing Thing { get; }
}
