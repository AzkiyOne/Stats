using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;

namespace Stats.Columns.ShearableDef;

public sealed class WoolAmountColumn<TRecord>(ColumnDef columnDef) : ThingDefCountColumn<TRecord, ThingDefCountColumnCell>(columnDef) where TRecord : IThingDefTableRecord
{
    protected override ThingDefCountColumnCell MakeCell(TRecord record)
    {
        CompProperties_Shearable? shearableCompProps = record.ShearableCompProperties;

        if (shearableCompProps != null)
        {
            Verse.ThingDef woolDef = shearableCompProps.woolDef;
            decimal woolAmount = shearableCompProps.woolAmount;

            return new ThingDefCountColumnCell(woolDef, woolAmount);
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Shearable>()?.woolDef)
            .Distinct();
    }
}
