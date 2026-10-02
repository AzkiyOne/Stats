using System.Collections.Generic;
using RimWorld;
using Stats.TableRecords;

namespace Stats.Columns.RangedWeaponDef;

public sealed class StatColumn<TRecord> : BuildableDef.StatColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public StatColumn(StatColumnDef def, List<TRecord> records, object _) : base(def, records, _)
    {
    }

    protected override StatRequest GetStatRequest(TRecord record)
    {
        Verse.ThingDef? turretGunDef = record.ThingDef.building?.turretGunDef;

        if (turretGunDef != null)
        {
            return StatRequest.For(turretGunDef, null);
        }

        return base.GetStatRequest(record);
    }
}
