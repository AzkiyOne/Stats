using RimWorld;

namespace Stats.TableRecords;

public interface IThingDefTableRecord : IBuildableDefTableRecord
{
    Verse.ThingDef ThingDef { get; }
}

public readonly record struct ThingDefTableRecord : IThingDefTableRecord
{
    public ThingDefTableRecord(Verse.ThingDef thingDef, Verse.ThingDef? stuffDef = null)
    {
        ThingDef = thingDef;
        StatRequest = StatRequest.For(thingDef, stuffDef);
    }

    public Verse.Def Def => ThingDef;

    public Verse.BuildableDef BuildableDef => ThingDef;

    public Verse.ThingDef ThingDef { get; }

    public StatRequest StatRequest { get; }
}
