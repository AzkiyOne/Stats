using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class DirectHitChanceColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public DirectHitChanceColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0\\%")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        VerbProperties? verbProps = record.ThingDef.GetGunPrimaryVerbProps();

        if (verbProps != null)
        {
            float directHitChance = verbProps.ForcedMissRadius > 0f
                ? 100f / GenRadial.NumCellsInRadius(verbProps.ForcedMissRadius)
                : 100f;

            return directHitChance.ToDecimal(1);
        }

        return 0m;
    }
}
