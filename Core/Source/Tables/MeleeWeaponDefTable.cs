using Verse;

namespace Stats.Tables;

public sealed class MeleeWeaponDefTable : ThingDefTableWorker
{
    public MeleeWeaponDefTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { IsMeleeWeapon: true, destroyOnDrop: false };
    }
}
