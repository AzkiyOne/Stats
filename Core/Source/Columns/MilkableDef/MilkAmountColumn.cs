using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Tables;

namespace Stats.Columns.MilkableDef;

public sealed class MilkAmountColumn<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumn<TRecord, ThingDefCountColumnCell>(columnDef)
        where TRecord :
            IMilkableDefTableRecord
{
    protected override ThingDefCountColumnCell MakeCell(TRecord record)
    {
        CompProperties_Milkable? milkableCompProps = record.MilkableCompProperties;

        if (milkableCompProps != null)
        {
            Verse.ThingDef milkDef = milkableCompProps.milkDef;
            decimal milkAmount = milkableCompProps.milkAmount;

            return new ThingDefCountColumnCell(milkDef, milkAmount);
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Milkable>()?.milkDef)
            .Distinct();
    }
}
