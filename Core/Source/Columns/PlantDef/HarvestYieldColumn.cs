using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Tables;
using UnityEngine;

namespace Stats.Columns.PlantDef;

public sealed class HarvestYieldColumn<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumn<TRecord, ThingDefCountColumnCell>(columnDef)
        where TRecord :
            IThingDefTableRecord
{
    protected override ThingDefCountColumnCell MakeCell(TRecord record)
    {
        PlantProperties plantProps = record.PlantProperties;

        if (plantProps is { harvestYield: > 0f, harvestedThingDef: not null })
        {
            decimal yield = Mathf.CeilToInt(plantProps.harvestYield);

            return new ThingDefCountColumnCell(plantProps.harvestedThingDef, yield);
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.plant?.harvestedThingDef)
            .Distinct();
    }
}
