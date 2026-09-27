using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Tables;

namespace Stats.Columns.BuildingDef;

public sealed class RecreationTypeColumn<TRecord>(ColumnDef columnDef) :
    DefColumn<TRecord, DefColumnCell>(columnDef)
        where TRecord :
            IBuildingDefTableRecord
{
    protected override DefColumnCell MakeCell(TRecord record)
    {
        BuildingProperties buildingProperties = record.BuildingProperties;
        JoyKindDef? joyKind = buildingProperties.joyKind;

        if (joyKind != null)
        {
            return new DefColumnCell(joyKind);
        }

        return default;
    }

    protected override IEnumerable<Verse.Def?> GetValueFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.building?.joyKind)
            .Distinct();
    }
}
