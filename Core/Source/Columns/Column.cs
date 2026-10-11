using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using UnityEngine;

namespace Stats.Columns;

public enum ColumnContentAlignment
{
    Left,
    Right,
    Middle,
}

// Interface used by table widget.
public abstract class Column
{
    private readonly string _label;
    private readonly Widget? _labelShort;
    private readonly string _description;

    protected Column(ColumnDef def) : this(def.LabelCap, def.labelShort, def.description)
    {
    }

    protected Column(string label, Widget? labelShort, string description)
    {
        _label = label;
        _labelShort = labelShort;
        _description = description;
    }

    public string Label => _label;

    public Widget? LabelShort => _labelShort;

    public string Description => _description;

    public abstract ColumnContentAlignment ContentAlignment { get; }

    public abstract ICollection<ColumnSortOption> SortOptions { get; }

    public abstract ICollection<ColumnFilterOption> FilterOptions { get; }

    public abstract bool AutoRefresh { get; }

    internal event Action? OnRefreshOnce;

    public abstract void DrawCell(Rect rect, int i);

    public virtual float GetMaxCellWidth(List<int> recordIds)
    {
        return recordIds.Select(GetCellWidth).Max();
    }

    protected abstract float GetCellWidth(int i);

    public abstract void RefreshCells();

    protected void RefreshOnce()
    {
        OnRefreshOnce?.Invoke();
    }

    public virtual void Dispose()
    {
    }
}

public abstract class Column<TRecord> : Column
{
    private readonly List<TRecord> _records;

    protected Column(ColumnDef def, List<TRecord> records) : this(def.LabelCap, def.labelShort, def.description, records)
    {
    }

    protected Column(string label, Widget? labelShort, string description, List<TRecord> records) : base(label, labelShort, description)
    {
        _records = records;
    }

    public abstract void Add(TRecord record);

    public abstract void Replace(int i1, int i2);

    public override void RefreshCells()
    {
        List<TRecord> records = _records;

        for (int i = 0; i < records.Count; i++)
        {
            TRecord record = records[i];

            RefreshCell(i, record);
        }
    }

    public abstract void RefreshCell(int i, TRecord record);
}
