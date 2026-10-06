using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Stats.Columns;
using Stats.Extensions;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

// There are two sides to this class:
// - Externally it is a list of TRecord.
// - Internally it is a SOA, where components are:
//   - Columns.
//   - List<TRecord>. So column[i] contains data related to records[i].
//
// Rows list acts as a mask and contains indexes of records, that have passed through
// current set of filters, in order defined by current sort settings.
//
// Lack of abstraction/leaking abstractions is (almost) intentional here.
// Because abstractions are not free.
public sealed partial class Table<TRecord> : TabBodyWidget
{
    // Filtering
    private readonly FiltersTab _filtersTab;
    private bool _isFiltersTabOpen;
    private bool _doFilter;

    // Sorting
    private ColumnWidget? _sortColumn;
    private int _sortDirection = SortDirectionAscending;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;
    private bool _doSort;

    // Rows
    private readonly List<TRecord> _records;
    // Why is this called "rows"?
    // Think of it as a list of "Row" objects with a single field - "tableRecordId" ("Id" being an "InDex").
    // Storing just id's as ints is a little bit more efficient.
    private readonly List<int> _rows;
    private int _topRowsCount;
    private int BottomRowsCount => _rows.Count - _topRowsCount;

    // Columns
    private readonly List<ColumnWidget> _columns;
    private int _leftColumnsCount;
    private int RightColumnsCount => _columns.Count - _leftColumnsCount;
    private ReadOnlyListSegment<ColumnWidget> LeftColumns => new(_columns, 0, _leftColumnsCount);
    private ReadOnlyListSegment<ColumnWidget> RightColumns => new(_columns, _leftColumnsCount, RightColumnsCount);
    private readonly Stack<ColumnWidget> _columnsToRefresh;

    // Layout
    private float _topRowsHeight;
    private float _bottomRowsHeight;
    private float _leftColumnsWidth;
    private Vector2 _contentSize;

    // Drawing
    private Vector2 _scrollPosition;
    // A way to defer any code that would otherwise modify
    // the collection that is currently being iterated over.
    // Primarily GUI event handlers.
    private Action? _beforeDraw;
    private bool _rightPartIsPanned;

    // Toolbar
    private readonly Toolbar _toolbar;

    // Misc
    private readonly DragManager<ColumnWidget> _dragManager;
    private readonly TableDef _def;

    public Table(TableDef def, int capacity, object[]? extraColumnCtorArgs = null) : this(def, new List<TRecord>(capacity), extraColumnCtorArgs)
    {
    }

    public Table(TableDef def, List<TRecord> records, object[]? extraColumnCtorArgs = null)
    {
        // Columns
        List<TableColumnListItem> columnList = def.columns;
        List<ColumnWidget> columns = new(columnList.Count);

        for (int i = 0; i < columnList.Count; i++)
        {
            TableColumnListItem columnListItem = columnList[i];
            ColumnDef columnDef = columnListItem.columnDef;

            try
            {
                Type columnType = columnDef.columnClass;
                if (columnType.IsGenericTypeDefinition)
                {
                    columnType = columnType.MakeGenericType(typeof(TRecord));
                }
                // TODO:
                // Should we pass initial visibility to column class?
                // Or maybe call Show/Hide (depends on what we choose as default state for columns).
                object[] columnCtorArgs = extraColumnCtorArgs == null
                    ? [columnDef, records]
                    : [columnDef, records, .. extraColumnCtorArgs];
                Column<TRecord> column = (Column<TRecord>)Activator.CreateInstance(columnType, columnCtorArgs);

                foreach (TRecord record in records)
                {
                    column.Add(record);
                }

                ColumnWidget columnWidget = new(column, columnListItem.isPrimary);

                columnWidget.OnPin += PinColumn;
                columnWidget.OnUnpin += UnpinColumn;
                columnWidget.OnRefreshOnce += RefreshColumnOnce;

                columns.Add(columnWidget);
            }
            catch (Exception error)
            {
                LogUnableToInitColumn(error, columnDef, def);
            }
        }

        // Misc
        HorDragManager<ColumnWidget> dragManager = new();
        dragManager.OnDragBefore += (ColumnWidget draggedColumn, ColumnWidget column) =>
            _beforeDraw ??= () => HandleColumnDrag(draggedColumn, column, true);
        dragManager.OnDragAfter += (ColumnWidget draggedColumn, ColumnWidget column) =>
            _beforeDraw ??= () => HandleColumnDrag(draggedColumn, column, false);

        // Toolbar
        Toolbar toolbar = new(columns);
        toolbar.OnFiltersButtonClick += () => _isFiltersTabOpen = !_isFiltersTabOpen;

        // Filters tab
        FiltersTab filtersTab = new(columns.SelectMany(column => column.FilterOptions));
        filtersTab.OnChange += () => _doFilter = true;

        // Finalize
        _records = records;
        _rows = [.. Enumerable.Range(0, records.Count)];
        _columns = columns;
        _columnsToRefresh = new Stack<ColumnWidget>(columns.Count);
        if (columns.Count > 0)
        {
            _leftColumnsCount = 1;
            _sortColumn = columns[0];
        }
        _toolbar = toolbar;
        _dragManager = dragManager;
        _def = def;
        _filtersTab = filtersTab;
    }

    public override void Resume()
    {
        // TODO?
    }

    public override void Suspend()
    {
        _rightPartIsPanned = false;
        _dragManager.EndDrag();

        for (int i = 0; i < _columns.Count; i++)
        {
            ColumnWidget column = _columns[i];

            column.Unfocus();
        }
    }

    public void AddRecord(TRecord record)
    {
        _records.Add(record);
        //_rows.Add(_records.Count - 1);

        foreach (ColumnWidget column in _columns)
        {
            try
            {
                column.AddRecord(record);
            }
            catch (Exception e)
            {
                LogUnableToAddRecordToColumn(e, record, column);
            }
        }
    }

    public void RemoveRecord(TRecord record)
    {
        int i = _records.IndexOf(record);

        _records.ReplaceWithLast(i);
        // _rows.Remove?

        foreach (ColumnWidget column in _columns)
        {
            try
            {
                column.RemoveRecord(i);
            }
            catch (Exception e)
            {
                LogUnableToRemoveRecordFromColumn(e, record, column);
            }
        }
    }

    public override void Dispose()
    {
        foreach (ColumnWidget column in _columns)
        {
            column.Dispose();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void LogUnableToInitColumn(Exception e, ColumnDef columnDef, TableDef def)
    {
        Log.Error($"Unable to initialize column \"{columnDef.defName}\" of table \"{def.defName}\": {e.Message}");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void LogUnableToAddRecordToColumn(Exception e, TRecord record, ColumnWidget column)
    {
        Log.Error($"Unable to add record \"{record}\" to \"{column.Def.defName}\" column of table \"{_def.defName}\": {e.Message}");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void LogUnableToRemoveRecordFromColumn(Exception e, TRecord record, ColumnWidget column)
    {
        Log.Error($"Unable to remove record \"{record}\" from \"{column.Def.defName}\" column of table \"{_def.defName}\": {e.Message}");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void LogUnableToRefreshColumn(Exception e, ColumnWidget column)
    {
        Log.Error($"Unable to refresh \"{column.Def.defName}\" column of \"{_def.defName}\" table: {e.Message}");
    }
}
