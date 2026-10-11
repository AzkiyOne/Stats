using Verse;

namespace Stats.Tables;

public sealed class MechanoidDefTable : ThingDefTableWorker
{
    public MechanoidDefTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { race.IsMechanoid: true, IsCorpse: false };
    }
}
