using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class RangeColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public RangeColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        VerbProperties? verbProps = record.ThingDef.GetGunPrimaryVerbProps();

        if (verbProps != null)
        {
            return verbProps.range.ToDecimal();
        }

        return 0m;
    }
}
