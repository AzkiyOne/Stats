using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class FertilityRequirementColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public FertilityRequirementColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0\\%")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        PlantProperties? plantProps = record.ThingDef.plant;

        if (plantProps?.fertilityMin > 0f)
        {
            return (100F * plantProps.fertilityMin).ToDecimal(1);
        }

        return 0m;
    }
}
