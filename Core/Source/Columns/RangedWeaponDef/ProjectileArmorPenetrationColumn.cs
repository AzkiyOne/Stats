using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class ProjectileArmorPenetrationColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public ProjectileArmorPenetrationColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0\\%")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        VerbProperties? verbProps = record.ThingDef.GetGunPrimaryVerbProps();
        ProjectileProperties? defaultProjProps = verbProps?.defaultProjectile?.projectile;

        if (defaultProjProps?.damageDef is { harmsHealth: true, armorCategory: not null })
        {
            float cellValue = defaultProjProps.GetArmorPenetration(null) * 100f;

            return cellValue.ToDecimal();
        }
        else if (defaultProjProps == null && verbProps?.beamDamageDef != null)
        {
            float cellValue = verbProps.beamDamageDef.defaultArmorPenetration * 100f;

            return cellValue.ToDecimal();
        }

        return 0m;
    }
}
