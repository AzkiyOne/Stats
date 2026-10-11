using RimWorld;
using Verse;

namespace Stats.Tables;

public sealed class RangedWeaponDefTable : ThingDefTableWorker
{
    public RangedWeaponDefTable(TableDef def) : base(def)
    {
    }

    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { IsRangedWeapon: true, destroyOnDrop: false }
               && thingDef.GetCompProperties<CompProperties_UniqueWeapon>() == null;
    }
}
