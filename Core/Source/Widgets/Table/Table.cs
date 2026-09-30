using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Columns;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

// Lack of abstraction/leaking abstractions is (almost) intentional here.
// Because abstractions are not free.
public sealed partial class Table<TRecord> : TabBodyWidget
{
    private static readonly TipSignal _manual =
        "- Hold (LMB) and move mouse cursor to scroll horizontally.\n" +
        "- Hold [Ctrl] and click on a column's name to pin/unpin it.\n" +
        "- Hold [Ctrl] and click on a row to pin/unpin it.\n" +
        "  - You can pin multiple rows.\n" +
        "  - Pinned rows are unaffected by filters.\n" +
        "- Pull top part of the window to change height.\n" +
        "- Double click to reset window height.";
    // Filtering
    //public override TableFilterMode FilterMode
    //{
    //    get => field;
    //    set
    //    {
    //        if (value == field) return;

    //        field = value;
    //        MatchRowCells = value switch
    //        {
    //            TableFilterMode.AND => MatchRowCells_AND,
    //            TableFilterMode.OR => MatchRowCells_OR,
    //            _ => throw new NotSupportedException("Unsupported table filtering mode.")
    //        };

    //        OnFilterModeChange?.Invoke(value);
    //        DoFilter = true;
    //    }
    //} = TableFilterMode.AND;
    //public override event Action<TableFilterMode>? OnFilterModeChange;
    //private readonly List<Filter> Filters;
    //private readonly HashSet<Filter> ActiveFilters;
    //private RowCellsMatcher MatchRowCells = MatchRowCells_AND;
    //private static readonly RowCellsMatcher MatchRowCells_AND =
    //(cells, filters) =>
    //{
    //    return filters.All(filter => filter.Widget.Eval(cells[filter.Column]));
    //};
    //private static readonly RowCellsMatcher MatchRowCells_OR =
    //(cells, filters) =>
    //{
    //    return filters.Any(filter => filter.Widget.Eval(cells[filter.Column]));
    //};

    // Sorting
    private ColumnWidget? _sortColumn;
    private int _sortDirection = SortDirectionAscending;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;

    // Filters tab

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
    private int _framesSinceLastFilterAndSort = 0;

    public Table(TableDef def, int capacity, object[]? extraColumnCtorArgs = null) : this(def, new List<TRecord>(capacity), extraColumnCtorArgs)
    {
    }

    public Table(TableDef def, List<TRecord> records, object[]? extraColumnCtorArgs = null)
    {
        // Columns
        List<ColumnDef> columnDefs = def.columns;
        List<ColumnWidget> columns = new(columnDefs.Count);

        for (int i = 0; i < columnDefs.Count; i++)
        {
            ColumnDef columnDef = columnDefs[i];

            try
            {
                Type columnType = columnDef.columnClass;
                if (columnType.IsGenericTypeDefinition)
                {
                    columnType = columnType.MakeGenericType(typeof(TRecord));
                }
                object[] columnCtorArgs = extraColumnCtorArgs == null
                    ? [columnDef, records]
                    : [columnDef, records, .. extraColumnCtorArgs];
                Column<TRecord> column = (Column<TRecord>)Activator.CreateInstance(columnType, columnCtorArgs);
                foreach (TRecord record in records)
                {
                    column.AddRecord(record);
                }
                ColumnWidget columnWidget = new(column);

                columnWidget.OnPin += PinColumn;
                columnWidget.OnUnpin += UnpinColumn;

                columns.Add(columnWidget);
            }
            catch (Exception error)
            {
                Log.Error(error.Message);
            }
        }

        // Misc
        HorDragManager<ColumnWidget> dragManager = new();
        dragManager.OnDragBefore += (ColumnWidget draggedColumn, ColumnWidget column) =>
            _beforeDraw ??= () => HandleColumnDrag(draggedColumn, column, true);
        dragManager.OnDragAfter += (ColumnWidget draggedColumn, ColumnWidget column) =>
            _beforeDraw ??= () => HandleColumnDrag(draggedColumn, column, false);

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
        _toolbar = new Toolbar(this);
        _dragManager = dragManager;
    }

    public override void Focus()
    {
        // TODO?
    }

    public override void Unfocus()
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
            column.AddRecord(record);
        }
    }

    public void RemoveRecord(TRecord record)
    {
        int i = _records.IndexOf(record);

        _records.RemoveAt(i);
        // _rows.Remove?

        foreach (ColumnWidget column in _columns)
        {
            column.RemoveRecord(i);
        }
    }

    public override void Dispose()
    {
        // TODO?
    }
}
