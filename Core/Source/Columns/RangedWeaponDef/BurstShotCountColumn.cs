using Stats.Extensions;
using Stats.TableRecords;
using System.Collections.Generic;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class BurstShotCountColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public BurstShotCountColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    protected override decimal GetValueFromRecord(TRecord record)
    {
        VerbProperties? verbProps = record.ThingDef.GetGunPrimaryVerbProps();

        if (verbProps is { Ranged: true, showBurstShotStats: true })
        {
            return verbProps.burstShotCount;
        }

        return 0m;
    }
}
