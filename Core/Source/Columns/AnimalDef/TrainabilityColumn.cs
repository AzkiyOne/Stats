using System.Collections.Generic;
using System.Linq;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Tables;
using Verse;

namespace Stats.Columns.AnimalDef;

public sealed class TrainabilityColumn<TRecord>(ColumnDef columnDef) :
    DefColumn<TRecord, DefColumnCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override DefColumnCell MakeCell(TRecord record)
    {
        RaceProperties raceProperties = record.RaceProperties;
        TrainabilityDef? trainability = raceProperties.trainability;

        if (trainability != null)
        {
            return new DefColumnCell(trainability);
        }

        return default;
    }

    protected override IEnumerable<Verse.Def?> GetValueFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.race?.trainability)
            .Distinct();
    }
}
