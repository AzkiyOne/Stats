using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class FertilitySensitivityColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public FertilitySensitivityColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records, "0\\%")
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        PlantProperties? plantProps = record.ThingDef.plant;

        if (plantProps?.fertilitySensitivity > 0f)
        {
            return (100f * plantProps.fertilitySensitivity).ToDecimal();
        }

        return 0m;
    }
}
