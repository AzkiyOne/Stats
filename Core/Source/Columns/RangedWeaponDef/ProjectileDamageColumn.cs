using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class ProjectileDamageColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public ProjectileDamageColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        VerbProperties? verbProperties = record.ThingDef.GetGunPrimaryVerbProps();
        ProjectileProperties? defaultProjProps = verbProperties?.defaultProjectile?.projectile;

        if (defaultProjProps?.damageDef?.harmsHealth == true)
        {
            Verse.ThingDef thingDef = record.ThingDef;
            decimal projectileDamage = defaultProjProps.GetDamageAmount(thingDef, null);

            return projectileDamage;
        }

        return 0m;
    }
}
