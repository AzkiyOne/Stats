using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class RPMColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public RPMColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0 rpm")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        VerbProperties? verbProps = record.ThingDef.GetGunPrimaryVerbProps();

        if (verbProps is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            // Reminder: This is not IRL RPM.
            float secondsBetweenShots = verbProps.ticksBetweenBurstShots.TicksToSeconds();
            float rpm = 60f / secondsBetweenShots;

            return rpm.ToDecimal();
        }

        return 0m;
    }
}
