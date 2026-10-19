using Stats.Extensions;
using Stats.TableRecords;
using System.Collections.Generic;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class MissRadiusColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public MissRadiusColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0")
    {
    }

    protected override decimal GetValueFromRecord(TRecord record)
    {
        VerbProperties? verbProps = record.ThingDef.GetGunPrimaryVerbProps();

        if (verbProps != null)
        {
            return verbProps.ForcedMissRadius.ToDecimal(1);
        }

        return 0m;
    }
}
