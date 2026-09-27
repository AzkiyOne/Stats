using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;

namespace Stats.Columns.ApparelDef.Reloadable;

public sealed class MaxChargesCountColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberColumnCell>(columnDef)
        where TRecord :
            IThingDefTableRecord
{
    protected override NumberColumnCell MakeCell(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        CompProperties_ApparelReloadable? reloadableCompProperties = thingDef.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties != null)
        {
            decimal maxCharges = reloadableCompProperties.maxCharges;

            return new NumberColumnCell(maxCharges);
        }

        return default;
    }
}
