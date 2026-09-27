using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Tables;

namespace Stats.Columns.RefuelableDef;

public sealed class FuelCapacityColumn<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumn<TRecord, ThingDefCountColumnCell>(columnDef)
        where TRecord :
            IRefuelableDefTableRecord
{
    protected override ThingDefCountColumnCell MakeCell(TRecord record)
    {
        CompProperties_Refuelable? refuelableCompProps = record.RefuelableCompProperties;

        if (refuelableCompProps != null)
        {
            Verse.ThingDef? fuelType = refuelableCompProps.fuelFilter?.AnyAllowedDef;

            if (fuelType != null)
            {
                float fuelCapacity = refuelableCompProps.fuelCapacity;

                return new ThingDefCountColumnCell(fuelType, fuelCapacity);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Refuelable>()?.fuelFilter?.AnyAllowedDef)
            .Distinct();
    }
}
