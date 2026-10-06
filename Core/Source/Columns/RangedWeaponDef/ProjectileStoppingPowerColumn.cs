using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class ProjectileStoppingPowerColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public ProjectileStoppingPowerColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        VerbProperties? verbProperties = record.ThingDef.GetGunPrimaryVerbProps();
        ProjectileProperties? defaultProjProps = verbProperties?.defaultProjectile?.projectile;

        if (defaultProjProps != null)
        {
            return defaultProjProps.stoppingPower.ToDecimal(1);
        }

        return 0m;
    }
}
