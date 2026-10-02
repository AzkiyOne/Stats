using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stats.Columns;

public enum ColumnContentAlignment
{
    Left,
    Right,
    Middle,
}

public abstract class Column<TRecord>
{
    // Records are passed here, NOT to fill a column with
    // values at the construction time (which will lead to calling abstract/virtual methods in constructor),
    // but so a column can implement lazy initialization.
    protected Column(ColumnDef def, List<TRecord> records)
    {
        Def = def;
        Records = records;
    }

    public ColumnDef Def { get; }

    protected List<TRecord> Records { get; }

    public abstract bool IsRefreshable { get; }

    public abstract ColumnContentAlignment ContentAlignment { get; }

    public abstract ICollection<ColumnSortOption> SortOptions { get; }

    public abstract ICollection<ColumnFilterOption> FilterOptions { get; }

    public abstract void DrawCell(Rect rect, int i);

    public virtual float GetMaxCellWidth(List<int> recordIds)
    {
        return recordIds.Select(GetCellWidth).Max();
    }

    protected abstract float GetCellWidth(int i);

    public abstract void Add(TRecord record);

    public abstract void Refresh(int i, TRecord record);

    public abstract void Remove(int i);

    public virtual void Show()
    {
    }

    public virtual void Hide()
    {
    }
}
