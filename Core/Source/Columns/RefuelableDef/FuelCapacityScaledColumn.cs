using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using UnityEngine;
using Verse;

namespace Stats.Columns.RefuelableDef;

public sealed class FuelCapacityScaledColumn<TRecord> : ThingDefCountColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public FuelCapacityScaledColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetFuelDefs(thingDefs))
    {
        Events.DifficultyChanged += RefreshOnce;
    }

    public override bool AutoRefresh => false;

    protected override ThingDefCount? GetThingDefCount(TRecord record)
    {
        CompProperties_Refuelable? refuelableCompProps = record.ThingDef.GetCompProperties<CompProperties_Refuelable>();

        if (refuelableCompProps != null)
        {
            Verse.ThingDef? fuelType = refuelableCompProps.fuelFilter?.AnyAllowedDef;

            if (fuelType != null && refuelableCompProps.FuelMultiplierCurrentDifficulty > 0f)
            {
                int fuelCapacity = Mathf.CeilToInt(refuelableCompProps.fuelCapacity / refuelableCompProps.FuelMultiplierCurrentDifficulty);

                return new ThingDefCount(fuelType, fuelCapacity);
            }
        }

        return null;
    }

    public override void Dispose()
    {
        base.Dispose();

        Events.DifficultyChanged -= RefreshOnce;
    }

    private static IEnumerable<Verse.ThingDef?> GetFuelDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Refuelable>()?.fuelFilter?.AnyAllowedDef)
            .Distinct();
    }
}
