using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Tables;
using UnityEngine;

namespace Stats.Columns.RefuelableDef;

// TODO:
//
// Realistically, we only need to refresh our cells once after difficulty settings had been changed.
//
// In order to do this:
// - Have a private _isStale flag that will be set to true after difficulty setting had been changed.
// - Override IsRefreshable and return the flag.
// - Override RefreshCells and set the flag to false at the end.
//
// Since we'll probably be subscribing to some global event, we'll need to have a Dispose method on base column worker class,
// so we can unsubscribe from the event when/if the column will be removed from a table.
public sealed class FuelCapacityScaledColumn<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumn<TRecord, FuelCapacityScaledColumn<TRecord>.TableCell>(columnDef)
        where TRecord :
            IRefuelableDefTableRecord
{
    protected override TableCell MakeCell(TRecord record)
    {
        CompProperties_Refuelable? refuelableCompProps = record.RefuelableCompProperties;

        if (refuelableCompProps != null)
        {
            Verse.ThingDef? fuelType = refuelableCompProps.fuelFilter?.AnyAllowedDef;

            if (fuelType != null)
            {
                decimal fuelCapacity = GetFuelCapacity(refuelableCompProps);

                return new TableCell(fuelType, fuelCapacity, refuelableCompProps);
            }
        }

        return default;
    }

    protected override TableCell RefreshCell(TableCell cell, out bool wasStale)
    {
        if (cell.RefuelableCompProps != null)
        {
            decimal fuelCapacity = GetFuelCapacity(cell.RefuelableCompProps);
            if (cell.Count != fuelCapacity)
            {
                wasStale = true;
                return new TableCell(cell.ThingDef, fuelCapacity, cell.RefuelableCompProps);
            }
        }

        wasStale = false;
        return cell;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Refuelable>()?.fuelFilter?.AnyAllowedDef)
            .Distinct();
    }

    private static decimal GetFuelCapacity(CompProperties_Refuelable refuelableCompProps)
    {
        // TODO: FuelMultiplierCurrentDifficulty might be 0
        return Mathf.CeilToInt(refuelableCompProps.fuelCapacity / refuelableCompProps.FuelMultiplierCurrentDifficulty);
    }

    public readonly struct TableCell : IThingDefCountCell
    {
        public Verse.ThingDef? ThingDef => _innerCell.ThingDef;
        public string ThingDefLabel => _innerCell.ThingDefLabel;
        public decimal Count => _innerCell.Count;
        public float MinWidth => _innerCell.MinWidth;
        public bool IsRefreshable => RefuelableCompProps != null;

        public readonly CompProperties_Refuelable? RefuelableCompProps;

        private readonly ThingDefCountColumnCell _innerCell;

        public TableCell(Verse.ThingDef fuelType, decimal fuelCapacity, CompProperties_Refuelable refuelableCompProps)
        {
            RefuelableCompProps = refuelableCompProps;
            _innerCell = new ThingDefCountColumnCell(fuelType, fuelCapacity);
        }

        public void Draw(Rect rect)
        {
            _innerCell.Draw(rect);
        }
    }
}
