using System.Collections.Generic;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.AnimalDef;

public sealed class NuzzleIntervalColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public NuzzleIntervalColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0.0 h")
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        RaceProperties? raceProps = record.ThingDef.race;

        if (raceProps != null)
        {
            float nuzzleInterval = raceProps.nuzzleMtbHours;

            if (nuzzleInterval > 0f)
            {
                return nuzzleInterval.ToDecimal(1);
            }
        }

        return 0m;
    }
}
