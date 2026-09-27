using RimWorld;
using Stats.Columns.Cells;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class GestationTimeColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberColumnCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberColumnCell MakeCell(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        float gestationTime = AnimalProductionUtility.GestationDaysLitter(thingDef);

        return new NumberColumnCell(gestationTime.ToDecimal(1), "0.0 d");
    }
}
