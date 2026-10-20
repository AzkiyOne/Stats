using Stats.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

// Look at this class as a DB table view, where filters are query and the source DB table is implemented outside.
public sealed partial class Table : IDrawable, IDisposable
{
    // Filtering
    private readonly FiltersTab _filtersTab;
    private bool _isFiltersTabOpen;
    private bool _doFilter;

    // Sorting
    private readonly SortWidget _sortWidget;
    private bool _doSortAllRows;
    private bool _doSortTopRows;

    // Rows
    // Why is this called "rows"?
    // Think of it as a list of "Row" objects with a single field - "tableRecordId" ("Id" being an "InDex").
    // Storing just id's as ints is a little bit more efficient.
    private readonly List<int> _topRows;
    private readonly List<int> _bottomRows;
    private int RowsCount => _topRows.Count + _bottomRows.Count;
    private int _recordsCount;

    // Columns
    private readonly List<ColumnWidget> _columns;
    private int _leftColumnsCount;
    private int RightColumnsCount => _columns.Count - _leftColumnsCount;
    private ReadOnlyListSegment<ColumnWidget> LeftColumns => new(_columns, 0, _leftColumnsCount);
    private ReadOnlyListSegment<ColumnWidget> RightColumns => new(_columns, _leftColumnsCount, RightColumnsCount);
    private readonly Queue<ColumnWidget> _columnsToRefresh;

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
    private Action? _postDraw;

    // Toolbar
    private readonly Toolbar _toolbar;

    // Misc
    private readonly ITableDataSource? _dataSource;
    private bool _forceTasks;

    public Table(IReadOnlyCollection<Column> columns, ITableDataSource dataSource) : this(columns, dataSource.RecordsCount)
    {
        _dataSource = dataSource;

        dataSource.OnAdd += Add;
        dataSource.OnReplace += Replace;
    }

    public Table(IReadOnlyCollection<Column> columns, int recordsCount)
    {
        // Columns
        List<ColumnWidget> columnWidgets = new(columns.Count);

        foreach (Column column in columns)
        {
            // Default visibility is set to "true" until i figure out a way to pass it down to
            // ColumnWidget from outside or be willing to expose the class to Table users.
            ColumnWidget columnWidget = new(column, true);

            columnWidget.OnTogglePin += ToggleColumnPin;

            columnWidgets.Add(columnWidget);
        }

        // Toolbar
        SortWidget sortWidget = new(columns.SelectMany(column => column.SortOptions));

        sortWidget.OnChange += () => _doSortAllRows = true;

        Toolbar toolbar = new(columnWidgets, sortWidget);

        toolbar.OnFiltersButtonClick += () => _isFiltersTabOpen = !_isFiltersTabOpen;

        // Filters tab
        FiltersTab filtersTab = new(columns.SelectMany(column => column.FilterOptions));

        filtersTab.OnChange += () => _doFilter = true;

        // Finalize
        _recordsCount = recordsCount;
        _topRows = new List<int>(10);
        _bottomRows = [.. Enumerable.Range(0, recordsCount)];
        _columns = columnWidgets;
        _columnsToRefresh = new Queue<ColumnWidget>(columns.Count);
        if (columns.Count > 0)
        {
            _leftColumnsCount = 1;
        }
        _sortWidget = sortWidget;
        _doSortAllRows = true;
        _toolbar = toolbar;
        _filtersTab = filtersTab;
    }

    public void Add()
    {
        _recordsCount++;
        _doFilter = true;
    }

    // Remember that removed record is swapped with
    // the last one and then it gets removed instead
    // (because it is faster that way).
    public void Replace(int recordIndex1, int recordIndex2)
    {
        //           Sync top rows
        //   Initial state | Replace(0, 4)
        //                 |
        //              Records
        //  0  1  2  3  4  |  0  1  2  3
        // [A, B, C, D, E] | [E, B, C, D]
        //                 |
        //             Both pinned
        //        A, C, E  |  C, E
        //       [0, 2, 4] | [2, 0]
        //             A pinned
        //           A, C  |  C
        //          [0, 2] | [2]
        //             E pinned
        //           E, C  |  E, C
        //          [4, 2] | [0, 2]
        int rowIndex1 = _topRows.IndexOf(recordIndex1);
        int rowIndex2 = _topRows.IndexOf(recordIndex2);

        if (rowIndex2 != -1)
        {
            _topRows[rowIndex2] = recordIndex1;
        }

        if (rowIndex1 != -1)
        {
            _topRows.RemoveAt(rowIndex1);
        }

        // For bottom rows we just do force reset to not make a record removal into (at least) O(n*2)
        _doFilter = true;
        _forceTasks = true;

        _recordsCount--;
    }

    public void Dispose()
    {
        _dataSource?.OnAdd -= Add;
        _dataSource?.OnReplace -= Replace;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void LogUnableToRefreshColumn(Exception e, ColumnWidget column)
    {
        Log.Error($"Unable to refresh \"{column.Label}\" column: {e.Message}");
    }
}
