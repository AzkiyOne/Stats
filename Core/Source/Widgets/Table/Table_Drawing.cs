using Stats.Extensions;
using Stats.GUIScopes;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using static Stats.GUIStyles.Table;

namespace Stats.Widgets;

public sealed partial class Table
{
    public void Draw(Rect rect)
    {
        EventType eventType = Event.current.type;

        if (eventType == EventType.Layout || _forceTasks)
        {
            // It is important to run tasks as early as possible,
            // so filtering can be done before _rows will be read by other code.
            // For example, by columns, to calculate their width.
            DoTasks();
            _forceTasks = false;
        }

        // Layout
        rect.CutTop(out Rect toolbarRect, GUIStyles.TableToolbar.Height)
            .CutLeft(out Rect filtersTabRect, _isFiltersTabOpen ? _filtersTab.Width : 0f)
            .TakeRest(out Rect tableRect);

        // Toolbar
        _toolbar.Draw(toolbarRect);

        if (_isFiltersTabOpen)
        {
            _filtersTab.Draw(filtersTabRect);
        }

        Rect viewportRect = tableRect;
        Rect contentRect = new(Vector2.zero, _contentSize);
        // Will scroll vertically
        if (_bottomRowsHeight > 0f)
        {
            viewportRect.width -= GenUI.ScrollBarWidth;
            // Add empty space for more convenient vertical scrolling.
            contentRect.height += viewportRect.height - HeadersRowHeight - _topRowsHeight;
        }
        // Will scroll horizontally
        if (contentRect.width > viewportRect.width)
        {
            viewportRect.height -= GenUI.ScrollBarWidth;
        }

        // Because of "sticky" columns and column headers we can't use scroll widget normally.
        // We only need to display two scrollbars tho, but i'm not in a mood to implement
        // what is basically a fake scroll widget. So we'll use the real one as background.
        using (new GUIScrollScope(tableRect, ref _scrollPosition, contentRect)) { }

        DrawVisibleContent(viewportRect);

        if (_postDraw != null)
        {
            _postDraw.Invoke();
            _postDraw = null;
        }
    }

    // TODO: We don't need 2 Draw() methods
    private void DrawVisibleContent(Rect rect)
    {
        EventType eventType = Event.current.type;

        // O(1) scroll content culling.
        // Since all rows have constant height, we can calculate:
        // - From what row/y to start drawing rows.
        // - How many rows to draw.
        Vector2 scrollPosition = _scrollPosition;
        float firstVisibleBottomRowY = -scrollPosition.y % RowHeight;
        int scrolledBottomRowsCount = Mathf.FloorToInt(scrollPosition.y / RowHeight);
        int bottomRowsLeftToScroll = _bottomRows.Count - scrolledBottomRowsCount;
        float bottomRowsRectHeight = rect.height - HeadersRowHeight - _topRowsHeight;
        int bottomRowsRectRowCapacity = Mathf.CeilToInt(bottomRowsRectHeight / RowHeight);
        int visibleBottomRowsCount = Math.Min(bottomRowsLeftToScroll, bottomRowsRectRowCapacity);
        if (visibleBottomRowsCount < 0)
        {
            visibleBottomRowsCount = 0;
        }

        GUIListSegment<int> topRows = new(_topRows);
        GUIListSegment<int> visibleBottomRows = new(_bottomRows, scrolledBottomRowsCount, visibleBottomRowsCount, firstVisibleBottomRowY);

        // Layout
        rect.CutLeft(out Rect leftColumnsRect, _leftColumnsWidth)
            .TakeRest(out Rect rightColumnsRect)
            .CutTop(HeadersRowHeight)// Register mouse-drag only below headers to not interfere with them.
            .TakeRest(out Rect mouseDragScrollAreaRect);

        // Rows
        DrawRows(rect, visibleBottomRows);

        // Pinned columns
        if (_leftColumnsCount > 0)
        {
            DrawColumns(leftColumnsRect, Vector2.zero, LeftColumns, topRows, visibleBottomRows);
            // Separator line
            if (eventType == EventType.Repaint)
            {
                leftColumnsRect.DrawBorderRight(FixedPartSeparatorLineColor);
            }
        }

        // Unpinned columns
        if (RightColumnsCount > 0)
        {
            using (new GUIClipScope(rightColumnsRect))
            {
                DrawColumns(rightColumnsRect with { x = 0f, y = 0f }, scrollPosition, RightColumns, topRows, visibleBottomRows);
            }
        }

        DoHorScrollControl(mouseDragScrollAreaRect);

        if (eventType == EventType.Layout)
        {
            RecalcLayout();
        }
    }

    private void DrawColumns(Rect rect, Vector2 scrollPosition, ReadOnlyListSegment<ColumnWidget> columns, GUIListSegment<int> topRows, GUIListSegment<int> bottomRows)
    {
        EventType eventType = Event.current.type;
        float xMin = rect.xMin;
        float xMax = rect.xMax;
        ref Rect columnRect = ref rect;
        columnRect.xMin -= scrollPosition.x;
        int columnsCount = columns.Length;
        ColumnWidget? draggedColumn = GUIUtils.DraggedObj as ColumnWidget;

        for (int i = 0; i < columnsCount; i++)
        {
            ColumnWidget column = columns[i];

            if (column.IsHidden == false)
            {
                columnRect.width = column.Width;
                float columnRectXmax = columnRect.xMax;

                if (columnRectXmax > xMin || eventType == EventType.Layout)
                {
                    column.Draw(columnRect, topRows, bottomRows);

                    if (draggedColumn != null)
                    {
                        HandleColumnDrag(draggedColumn, column, columnRect);
                    }
                }

                if (columnRectXmax > xMax && eventType != EventType.Layout)
                {
                    break;
                }

                columnRect.x = columnRectXmax;
            }
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void HandleColumnDrag(ColumnWidget draggedColumn, ColumnWidget column, Rect columnRect)
    {
        float mouseX = Event.current.mousePosition.x;
        FloatRange horBoundaries = columnRect.HorBoundaries();

        if (horBoundaries.MinHalf().Includes(mouseX))
        {
            _postDraw ??= () => MoveColumn(draggedColumn, column, true);
        }
        else if (horBoundaries.MaxHalf().Includes(mouseX))
        {
            _postDraw ??= () => MoveColumn(draggedColumn, column, false);
        }

        if (draggedColumn == column && Event.current.type == EventType.Repaint)
        {
            columnRect.HighlightActive();
        }
    }

    private void DrawRows(Rect rect, GUIListSegment<int> visibleBottomRows)
    {
        bool isRepaint = Event.current.type == EventType.Repaint;

        // Layout
        rect.CutTop(out Rect headersRowRect, HeadersRowHeight)
            .CutTop(out Rect topRowsRect, _topRowsHeight)
            .TakeRest(out Rect bottomRowsRect);

        // Headers row
        if (isRepaint)
        {
            headersRowRect.Fill(HeadersRowBGColor)
                          .DrawBorderBottom(ColumnSeparatorLineColor);
        }

        // Pinned rows
        int topRowsCount = _topRows.Count;

        if (topRowsCount > 0)
        {
            if (isRepaint)
            {
                topRowsRect.DrawBorderBottom(FixedPartSeparatorLineColor);
            }

            // Rows
            Rect rowRect = topRowsRect with { height = RowHeight };

            for (int i = 0; i < topRowsCount; i++)
            {
                DrawRow(rowRect, i, true);

                rowRect.y = rowRect.yMax;
            }
        }

        // Unpinned rows
        // 
        // This part uses manual clipping.
        // Why? Because i can.
        if (visibleBottomRows.Length > 0)
        {
            float rectYmax = rect.yMax;
            float firstRowHeight = RowHeight + visibleBottomRows.AxisStart;
            Rect rowRect = bottomRowsRect with { height = firstRowHeight };

            for (int i = visibleBottomRows.Start; i < visibleBottomRows.End; i++)
            {
                DrawRow(rowRect, i, false);

                rowRect.y = rowRect.yMax;
                rowRect.height = RowHeight;

                if (rowRect.yMax > rectYmax)
                {
                    rowRect.yMax = rectYmax;
                }
            }
        }
    }

    private void DrawRow(Rect rect, int index, bool isPinned)
    {
        bool mouseIsOverRect = Mouse.IsOver(rect);
        Event curEvent = Event.current;

        if (curEvent.type == EventType.Repaint)
        {
            if (mouseIsOverRect)
            {
                rect.Highlight();
            }
            else if (index % 2 == 0)
            {
                rect.HighlightLight();
            }
        }

        if (mouseIsOverRect && curEvent is { type: EventType.MouseUp, button: 0, modifiers: EventModifiers.Control })
        {
            _postDraw = () => ToggleRowPin(index, isPinned);
            curEvent.Use();
        }
    }

    private void DoHorScrollControl(Rect rect)
    {
        GUIUtils.DragState dragState = rect.DoDraggableControl(this);

        if (dragState == GUIUtils.DragState.Dragged && Event.current.type == EventType.Layout)
        {
            _scrollPosition.x = Mathf.Max(_scrollPosition.x - GUIUtils.MouseDelta.x, 0f);
        }
    }

    private readonly Stopwatch _stopwatch = new();

    private void DoTasks()
    {
        // How it works:
        //
        // The table is constantly running 3 tasks:
        // - Filtering
        // - Sorting
        // - Column refreshing
        //
        // Only one task is executed per frame.
        // Filtering and sorting take priority.
        // 
        // Example.
        //
        // All columns are stale.
        // N - frame number.
        //
        // 0: ColumnsToRefresh = [A, B, C]
        // 1: Refresh column A
        // 2: Apply filters, Sort
        // 3: Refresh column B
        // 4: Apply filters, Sort
        // 5: Filters were changed by the player
        // 6: Apply filters, Sort
        // 7: Sorting settings were changed by the player
        // 8: Sort
        // 9: Refresh column C
        // 10: ColumnsToRefresh = [A, B, C]
        if (_doFilter)
        {
            ApplyFilters();

            _doFilter = false;
            // We have to do sorting immediately, otherwise the table will flicker.
            _sortWidget.SortRows(_bottomRows);
        }
        else if (_doSortAllRows)
        {
            _sortWidget.SortRows(_topRows);
            _sortWidget.SortRows(_bottomRows);

            _doSortAllRows = false;
            _doSortTopRows = false;
        }
        else if (_doSortTopRows)
        {
            _sortWidget.SortRows(_topRows);

            _doSortTopRows = false;
        }
        else if (_columnsToRefresh.Count > 0)
        {
            _stopwatch.Start();

            while (_columnsToRefresh.TryDequeue(out ColumnWidget? column) && _stopwatch.ElapsedMilliseconds < 1)
            {
                // TODO: Implementation
                //bool columnWasAltered = false;
                bool columnWasAltered = true;

                try
                {
                    column.RefreshCells();
                }
                catch (Exception e)
                {
                    LogUnableToRefreshColumn(e, column);
                }

                if (columnWasAltered)
                {
                    _doFilter = true;
                    _doSortTopRows = true;
                }
            }

            _stopwatch.Reset();
        }
        else
        {
            for (int i = 0; i < _columns.Count; i++)
            {
                ColumnWidget column = _columns[i];

                if (column.AutoRefresh)
                {
                    _columnsToRefresh.Enqueue(column);
                }
            }
        }
    }
}
