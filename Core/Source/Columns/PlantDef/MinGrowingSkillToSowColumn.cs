using System.Collections.Generic;
using RimWorld;
using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class MinGrowingSkillToSowColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public MinGrowingSkillToSowColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        PlantProperties? plantProperties = record.ThingDef.plant;

        if (plantProperties != null)
        {
            return plantProperties.sowMinSkill;
        }

        return 0m;
    }
}
