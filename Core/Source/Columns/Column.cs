using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats.Columns;

public enum ColumnContentAlignment
{
    Left,
    Right,
    Middle,
}

public abstract class Column
{
    protected Column(ColumnDef def)
    {
        Def = def;
    }

    public ColumnDef Def { get; }

    public abstract ColumnContentAlignment ContentAlignment { get; }

    public abstract ICollection<ColumnSortOption> SortOptions { get; }

    public abstract ICollection<ColumnFilterOption> FilterOptions { get; }

    public abstract bool IsRefreshable { get; }

    public abstract void DrawCell(Rect rect, int i);

    public virtual float GetMinWidth(List<int> recordIds)
    {
        return recordIds.Select(GetCellWidth).Max();
    }

    protected abstract float GetCellWidth(int i);

    public abstract void RefreshCells();

    public virtual void Hide()
    {
    }
}

public abstract class Column<TRecord> : Column
{
    // Records are passed here, NOT to fill a column with
    // values at the construction time (which will lead to calling abstract/virtual methods in constructor),
    // but so a column can implement lazy initialization.
    protected Column(ColumnDef def, List<TRecord> records) : base(def)
    {
        Records = records;
    }

    protected List<TRecord> Records { get; }

    public abstract void AddRecord(TRecord record);

    public abstract void RemoveRecord(int i);
}

public abstract class Column<TRecord, TValue> : Column<TRecord>
{
    protected Column(ColumnDef def, List<TRecord> records) : base(def, records)
    {
    }

    protected abstract TValue GetValueFromRecord(TRecord record);

    protected abstract void AddValue(TValue value);

    protected virtual void AddValue(Exception exception)
    {
        AddValue(default(TValue));
    }

    protected abstract void SetValue(int i, TValue value);

    protected virtual void SetValue(int i, Exception exception)
    {
        SetValue(i, default(TValue));
    }

    protected abstract void RemoveValue(int i);

    public override void AddRecord(TRecord record)
    {
        try
        {
            TValue value = GetValueFromRecord(record);

            AddValue(value);
        }
        catch (Exception exception)
        {
            AddValue(exception);

            Log.Error($"Unable to add record {record} to {Def.defName}: {exception.Message}");
        }
    }

    public override void RemoveRecord(int i)
    {
        RemoveValue(i);
    }

    public override void RefreshCells()
    {
        for (int i = 0; i < Records.Count; i++)
        {
            TRecord record = Records[i];

            try
            {
                TValue value = GetValueFromRecord(record);

                SetValue(i, value);
            }
            catch (Exception exception)
            {
                SetValue(i, exception);

                Log.Error($"Unable to update record {record} of {Def.defName}: {exception.Message}");
            }
        }
    }
}
