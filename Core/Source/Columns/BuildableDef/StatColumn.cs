using System.Collections.Generic;
using RimWorld;
using Stats.Columns;
using Stats.Defs;
using Stats.TableRecords;
using Stats.Widgets.Filters;

namespace Stats.Columns.BuildableDef;

public class StatColumn<TRecord> : Column<TRecord, StatColumnCell> where TRecord : IBuildableDefTableRecord
{
    private readonly StatDef _statDef;
    private readonly string _label;

    public StatColumn(StatColumnDef def) : base(def)
    {
        _statDef = def.stat;
        _label = def.LabelCap;
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Right;

    public override ICollection<ColumnSortOption> SortOptions => [
        new ColumnSortOption(_label, (i1, i2) => this[i1].StatValue.CompareTo(this[i2].StatValue))
    ];

    public override ICollection<ColumnFilterOption> FilterOptions => [
        new ColumnFilterOption(_label, () => new NumberFilter(i => this[i].Value))
    ];

    protected override StatColumnCell MakeCell(TRecord record)
    {
        StatRequest statRequest = GetStatRequest(record);

        if (_statDef.Worker.ShouldShowFor(statRequest))
        {
            float statValue = _statDef.Worker.GetValue(statRequest);

            return new StatColumnCell(statValue, statRequest, _statDef);
        }

        return default;
    }

    protected virtual StatRequest GetStatRequest(TRecord record)
    {
        return record.StatRequest;
    }

    protected override bool IsRefreshable => typeof(TRecord) is IThingTableRecord;

    protected override void RefreshCell(TRecord record, StatColumnCell cell, int i)
    {
        StatRequest statRequest = GetStatRequest(record);
        float newStatValue = _statDef.Worker.GetValue(statRequest);

        if (newStatValue != cell.StatValue)
        {
            this[i] = new StatColumnCell(newStatValue, statRequest, _statDef);
        }
    }
}
