using Verse;

namespace Stats.Tables;

public sealed class AnimalDefTable : ThingDefTableWorker
{
    public AnimalDefTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { race.Animal: true, IsCorpse: false };
    }
}
