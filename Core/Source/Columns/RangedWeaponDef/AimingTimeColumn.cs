using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RangedWeaponDef;

public sealed class AimingTimeColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    private static readonly string _formatString = "0.00 " + "LetterSecond".Translate();

    public AimingTimeColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, _formatString)
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        VerbProperties? verbProps = record.ThingDef.GetGunPrimaryVerbProps();

        if (verbProps != null)
        {
            return verbProps.warmupTime.ToDecimal(2);
        }

        return 0m;
    }
}
