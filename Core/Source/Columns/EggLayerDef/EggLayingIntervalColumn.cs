using RimWorld;
using Stats.Columns.Cells;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.EggLayerDef;

public sealed class EggLayingIntervalColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberColumnCell>(columnDef)
        where TRecord :
            IEggLayerDefTableRecord
{
    protected override NumberColumnCell MakeCell(TRecord record)
    {
        CompProperties_EggLayer? eggLayerCompProps = record.EggLayerCompProperties;

        if (eggLayerCompProps != null)
        {
            float eggLayingInterval = eggLayerCompProps.eggLayIntervalDays;

            return new NumberColumnCell(eggLayingInterval.ToDecimal(1), "0.0 d");
        }

        return default;
    }
}
