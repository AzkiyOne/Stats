using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using UnityEngine;
using Verse;

namespace Stats.Columns.PlantDef;

public sealed class HarvestYieldColumn<TRecord> : ThingDefCountColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public HarvestYieldColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetHarvestedThingDefs(thingDefs))
    {
    }

    public override bool IsRefreshable => false;

    protected override ThingDefCount? GetValueFromRecord(TRecord record)
    {
        PlantProperties? plantProps = record.ThingDef.plant;

        if (plantProps is { harvestYield: > 0f, harvestedThingDef: not null })
        {
            int yield = Mathf.CeilToInt(plantProps.harvestYield);

            return new ThingDefCount(plantProps.harvestedThingDef, yield);
        }

        return null;
    }

    private static IEnumerable<Verse.ThingDef?> GetHarvestedThingDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .Select(thingDef => thingDef.plant?.harvestedThingDef)
            .Distinct();
    }
}
