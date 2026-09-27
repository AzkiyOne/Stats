using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class GrowthTimeColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberColumnCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberColumnCell MakeCell(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        float growthTime = AnimalProductionUtility.DaysToAdulthood(thingDef);

        return new NumberColumnCell(growthTime, "0 d");
    }
}
