using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Tables;

namespace Stats.Columns.ApparelDef;

public sealed class LayersColumn<TRecord>(ColumnDef columnDef) :
    DefSetColumn<TRecord, DefSetColumnCell>(columnDef)
        where TRecord :
            IApparelDefTableRecord
{
    protected override DefSetColumnCell MakeCell(TRecord record)
    {
        ApparelProperties apparelProps = record.ApparelProperties;
        List<Verse.ApparelLayerDef> layers = apparelProps.layers;

        return new DefSetColumnCell(layers);
    }

    protected override IEnumerable<Verse.Def?> GetValueFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .SelectMany(thingDef => thingDef.apparel?.layers)
            .Distinct();
    }
}
