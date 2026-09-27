using RimWorld;
using Stats.Columns.Cells;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class AverageLitterSizeColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberColumnCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberColumnCell MakeCell(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        float averageLitterSize = AnimalProductionUtility.OffspringRange(thingDef).Average;

        return new NumberColumnCell(averageLitterSize.ToDecimal(1), "0.0");
    }
}
