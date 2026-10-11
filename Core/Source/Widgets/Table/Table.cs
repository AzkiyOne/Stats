using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Stats.Columns;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

// Look at this class as a DB table view, where filters are query and the source DB table is implemented outside.
public sealed partial class Table : IDrawable, ISuspendable, IDisposable
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
    private readonly Queue<ColumnWidget> _columnsToRefresh;
    private readonly HashSet<ColumnWidget> _columnsToQueueOnNextCycle;

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

    public Table(IReadOnlyCollection<Column> columns, int recordsCount)
    {
        // Columns
        List<ColumnWidget> columnWidgets = new(columns.Count);

        foreach (Column column in columns)
        {
            // Default visibility is set to "true" until i figure out a way to pass it down to
            // ColumnWidget from outside or be willing to expose the class to Table users.
            ColumnWidget columnWidget = new(column, true);

            columnWidget.OnPin += PinColumn;
            columnWidget.OnUnpin += UnpinColumn;
            columnWidget.OnRefreshOnce += RefreshColumnOnce;

            columnWidgets.Add(columnWidget);
        }

        // Misc
        HorDragManager<ColumnWidget> dragManager = new();
        dragManager.OnDragBefore += (ColumnWidget draggedColumn, ColumnWidget column) =>
            _beforeDraw ??= () => HandleColumnDrag(draggedColumn, column, true);
        dragManager.OnDragAfter += (ColumnWidget draggedColumn, ColumnWidget column) =>
            _beforeDraw ??= () => HandleColumnDrag(draggedColumn, column, false);

        // Toolbar
        Toolbar toolbar = new(columnWidgets);
        toolbar.OnFiltersButtonClick += () => _isFiltersTabOpen = !_isFiltersTabOpen;

        // Filters tab
        FiltersTab filtersTab = new(columns.SelectMany(column => column.FilterOptions));
        filtersTab.OnChange += () => _doFilter = true;

        // Finalize
        _rows = [.. Enumerable.Range(0, recordsCount)];
        _columns = columnWidgets;
        _columnsToRefresh = new Queue<ColumnWidget>(columns.Count);
        _columnsToQueueOnNextCycle = new();
        if (columns.Count > 0)
        {
            _leftColumnsCount = 1;
            //_sortColumn = columns[0];
        }
        _toolbar = toolbar;
        _dragManager = dragManager;
        _filtersTab = filtersTab;
    }

    public void Resume()
    {
    }

    public void Suspend()
    {
        _rightPartIsPanned = false;
        _dragManager.EndDrag();

        for (int i = 0; i < _columns.Count; i++)
        {
            ColumnWidget column = _columns[i];

            column.Suspend();
        }
    }

    public void Dispose()
    {
        foreach (ColumnWidget column in _columns)
        {
            column.Dispose();
        }
    }

    //[MethodImpl(MethodImplOptions.NoInlining)]
    //private static void LogUnableToInitColumn(Exception e, ColumnDef columnDef, TableDef def)
    //{
    //    Log.Error($"Unable to initialize column \"{columnDef.defName}\" of table \"{def.defName}\": {e.Message}");
    //}

    //[MethodImpl(MethodImplOptions.NoInlining)]
    //private void LogUnableToAddRecordToColumn(Exception e, TRecord record, ColumnWidget column)
    //{
    //    Log.Error($"Unable to add record \"{record}\" to \"{column.Def.defName}\" column of table \"{_def.defName}\": {e.Message}");
    //}

    //[MethodImpl(MethodImplOptions.NoInlining)]
    //private void LogUnableToRemoveRecordFromColumn(Exception e, TRecord record, ColumnWidget column)
    //{
    //    Log.Error($"Unable to remove record \"{record}\" from \"{column.Def.defName}\" column of table \"{_def.defName}\": {e.Message}");
    //}

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void LogUnableToRefreshColumn(Exception e, ColumnWidget column)
    {
        Log.Error($"Unable to refresh \"{column.Label}\" column: {e.Message}");
    }
}
