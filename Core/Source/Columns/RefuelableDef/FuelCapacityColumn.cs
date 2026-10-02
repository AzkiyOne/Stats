using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.RefuelableDef;

public sealed class FuelCapacityColumn<TRecord> : ThingDefCountColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public FuelCapacityColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetFuelDefs(thingDefs))
    {
    }

    public override bool IsRefreshable => false;

    protected override ThingDefCount? GetThingDefCount(TRecord record)
    {
        CompProperties_Refuelable? refuelableCompProps = record.ThingDef.GetCompProperties<CompProperties_Refuelable>();

        if (refuelableCompProps != null)
        {
            Verse.ThingDef? fuelType = refuelableCompProps.fuelFilter?.AnyAllowedDef;

            if (fuelType != null)
            {
                float fuelCapacity = refuelableCompProps.fuelCapacity;

                return new ThingDefCount(fuelType, (int)fuelCapacity);
            }
        }

        return null;
    }

    private static IEnumerable<Verse.ThingDef?> GetFuelDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Refuelable>()?.fuelFilter?.AnyAllowedDef)
            .Distinct();
    }
}
