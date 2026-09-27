using CombatExtended;
using RimWorld;
using Stats.TableRecords.ThingDef;
using Stats.ObjectTable.ColumnWorkers;
using Verse;
using Stats.Extensions;

namespace Stats.Compat.CE;

public sealed class Weapon_ReloadTimeColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Weapon_ReloadTimeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00 " + "LetterSecond".Translate())
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var statRequest = StatRequest.For(thingDef, null);

        if (CE_StatDefOf.MagazineCapacity.Worker.ShouldShowFor(statRequest))
        {
            return CE_StatDefOf.ReloadTime.Worker.GetValue(statRequest).ToDecimal(2);
        }

        return 0m;
    }
}
