using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.PawnDef;

public sealed class CaravanCarryingCapacityColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public CaravanCarryingCapacityColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0 kg")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        RaceProperties? raceProperties = record.ThingDef.race;

        if (raceProperties != null)
        {
            float baseBodySize = raceProperties.baseBodySize;
            float caravanCarryingCapacity = baseBodySize * MassUtility.MassCapacityPerBodySize;

            return caravanCarryingCapacity.ToDecimal();
        }

        return 0m;
    }
}
