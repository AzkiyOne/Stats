using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class ProjectileBDFImpassableColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public ProjectileBDFImpassableColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0\\%")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        VerbProperties? verbProperties = record.ThingDef.GetGunPrimaryVerbProps();
        DamageDef? defaultProjDamageDef = verbProperties?.defaultProjectile?.projectile?.damageDef;

        if (defaultProjDamageDef != null)
        {
            float projectileBDFImpassable = defaultProjDamageDef.buildingDamageFactorImpassable * 100f;

            return projectileBDFImpassable.ToDecimal();
        }

        return 0m;
    }
}
