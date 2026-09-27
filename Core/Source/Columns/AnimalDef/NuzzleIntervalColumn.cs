using Stats.Columns.Cells;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.AnimalDef;

public sealed class NuzzleIntervalColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberColumnCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override NumberColumnCell MakeCell(TRecord record)
    {
        RaceProperties raceProps = record.RaceProperties;
        float nuzzleInterval = raceProps.nuzzleMtbHours;

        if (nuzzleInterval > 0f)
        {
            return new NumberColumnCell(nuzzleInterval.ToDecimal(1), "0.0 h");
        }

        return default;
    }
}
