using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.PawnDef;

public sealed class LifeExpectancyColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public LifeExpectancyColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0 y")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        RaceProperties? raceProperties = record.ThingDef.race;

        if (raceProperties != null)
        {
            return raceProperties.lifeExpectancy.ToDecimal();
        }

        return 0m;
    }
}
