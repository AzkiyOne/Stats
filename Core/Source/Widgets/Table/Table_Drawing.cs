using System;
using Stats.Extensions;
using Stats.GUIScopes;
using UnityEngine;
using Verse;
using static Stats.GUIStyles.Table;

namespace Stats.Widgets;

public sealed partial class Table<TRecord>
{
    public override void Draw(Rect rect)
    {
        if (_beforeDraw != null)
        {
            _beforeDraw.Invoke();
            _beforeDraw = null;
        }

        // Layout
        rect.CutTop(out Rect toolbarRect, GUIStyles.TableToolbar.Height)
            .TakeRest(out Rect tableRect);

        // Toolbar
        _toolbar.Draw(toolbarRect);

        //if (showSettingsMenu)
        //{
        //    DrawColumnsTab(ref rect);
        //}

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
            contentRect.height -= GenUI.ScrollBarWidth;
        }

        // Because of "sticky" columns and column headers we can't use scroll widget normally.
        // We only need to display two scrollbars tho, but i'm not in a mood to implement
        // what is basically a fake scroll widget. So we'll use the real one as background.
        using (new GUIScrollScope(tableRect, ref _scrollPosition, contentRect)) { }

        DrawVisibleContent(viewportRect);
    }

    // TODO: We don't need 2 Draw() methods
    private void DrawVisibleContent(Rect rect)
    {
        EventType eventType = Event.current.type;

        if (eventType == EventType.Layout)
        {
            _framesSinceLastFilterAndSort++;

            if (_columnsToRefresh.Count > 0)
            {
                ColumnWidget column = _columnsToRefresh.Pop();

                try
                {
                    column.RefreshCells(_records);
                }
                catch (Exception e)
                {
                    LogUnableToRefreshColumn(e, column);
                }
            }
            // Filtering and sorting of rows is performed only after every column has been refreshed, but:
            // - No often than every 60 frames.
            // - Regardless of how many refreshable columns there are.
            else if (_framesSinceLastFilterAndSort >= 60)
            {
                // Filter
                // It is important to filter rows as early as possible because
                // they'll be then used by columns to calculate their width.

                // Sort

                // Reset
                foreach (ColumnWidget column in _columns)
                {
                    if (column.IsRefreshable)
                    {
                        _columnsToRefresh.Push(column);
                    }
                }

                _framesSinceLastFilterAndSort = 0;
            }
        }

        // O(1) scroll content culling.
        // Since all rows have constant height, we can calculate:
        // - From what row/y to start drawing rows.
        // - How many rows to draw.
        Vector2 scrollPosition = _scrollPosition;
        float firstVisibleBottomRowY = -scrollPosition.y % RowHeight;
        int scrolledBottomRowsCount = Mathf.FloorToInt(scrollPosition.y / RowHeight);
        int bottomRowsLeftToScroll = BottomRowsCount - scrolledBottomRowsCount;
        float bottomRowsRectHeight = rect.height - HeadersRowHeight - _topRowsHeight;
        int bottomRowsRectRowCapacity = Mathf.CeilToInt(bottomRowsRectHeight / RowHeight);
        int visibleBottomRowsCount = Math.Min(bottomRowsLeftToScroll, bottomRowsRectRowCapacity);
        // There was a bug where this value was evaluating to negative int which caused CTD when
        // int[] was being allocated on a stack below.
        // Although i couldn't reproduce it in recent testing, i'll leave this check just in case.
        // The math for this value is not very precise (lots of divisions, rounding and float values are involved).
        if (visibleBottomRowsCount < 0)
        {
            visibleBottomRowsCount = 0;
        }
        int topRowsCount = _topRowsCount;

        int visibleBottomRowsStart = topRowsCount + scrolledBottomRowsCount;
        Span<int> visibleBottomRows = stackalloc int[visibleBottomRowsCount];
        _rows.CopyTo(visibleBottomRows, visibleBottomRowsStart);

        Span<int> topRows = stackalloc int[topRowsCount];
        _rows.CopyTo(topRows);

        // Layout
        rect.CutLeft(out Rect leftColumnsRect, _leftColumnsWidth)
            .TakeRest(out Rect rightColumnsRect)
            .CutTop(HeadersRowHeight)// Register mouse-drag only below headers to not interfere with them.
            .TakeRest(out Rect mouseDragScrollAreaRect);

        // Rows
        DrawRows(rect, visibleBottomRowsStart, visibleBottomRowsCount, firstVisibleBottomRowY);

        // Pinned columns
        if (_leftColumnsCount > 0)
        {
            DrawColumns(leftColumnsRect, Vector2.zero, LeftColumns, topRows, visibleBottomRows, firstVisibleBottomRowY);
            // Separator line
            if (eventType == EventType.Repaint)
            {
                leftColumnsRect.DrawBorderRight(FixedPartSeparatorLineColor);
            }
        }

        // Unpinned columns
        if (RightColumnsCount > 0)
        {
            using (new GUIClipScope(rightColumnsRect, new Vector2(-scrollPosition.x, 0f)))
            {
                DrawColumns(rightColumnsRect with { x = 0f, y = 0f }, scrollPosition, RightColumns, topRows, visibleBottomRows, firstVisibleBottomRowY);
            }
        }

        DoHorScrollControl(mouseDragScrollAreaRect);

        if (eventType == EventType.Layout)
        {
            RecalcLayout();
        }
    }

    private void DrawColumns(Rect rect, Vector2 scrollPosition, ReadOnlyListSegment<ColumnWidget> columns, Span<int> topRows, Span<int> bottomRows, float bottomRowsY)
    {
        EventType eventType = Event.current.type;
        float scrollX = scrollPosition.x;
        float xMin = rect.xMin + scrollX;
        float xMax = rect.xMax + scrollX;
        ref Rect columnRect = ref rect;
        int columnsCount = columns.Length;

        for (int i = 0; i < columnsCount; i++)
        {
            ColumnWidget column = columns[i];

            if (column.IsHidden == false)
            {
                columnRect.width = column.Width;
                float columnRectXmax = columnRect.xMax;

                if (columnRectXmax > xMin || eventType == EventType.Layout)
                {
                    column.Draw(columnRect, _rows, topRows, bottomRows, bottomRowsY, _dragManager);
                }

                if (columnRectXmax > xMax && eventType != EventType.Layout)
                {
                    break;
                }

                columnRect.x = columnRectXmax;
            }
        }
    }

    private void DrawRows(Rect rect, int bottomRowsStart, int bottomRowsCount, float bottomRowsY)
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
        int topRowsCount = _topRowsCount;
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
                DrawRow(rowRect, i);
                rowRect.y = rowRect.yMax;
            }
        }

        // Unpinned rows
        // 
        // This part uses manual clipping.
        // Why? Because i can.
        if (bottomRowsCount > 0)
        {
            float rectYmax = rect.yMax;
            float firstRowHeight = RowHeight + bottomRowsY;
            int bottomRowsEnd = bottomRowsCount + bottomRowsStart;// Exclusive
            Rect rowRect = bottomRowsRect with { height = firstRowHeight };

            for (int i = bottomRowsStart; i < bottomRowsEnd; i++)
            {
                DrawRow(rowRect, i);

                rowRect.y = rowRect.yMax;
                rowRect.height = RowHeight;

                if (rowRect.yMax > rectYmax)
                {
                    rowRect.yMax = rectYmax;
                }
            }
        }
    }

    private void DrawRow(Rect rect, int index)
    {
        bool mouseIsOverRect = Mouse.IsOver(rect);
        Event @event = Event.current;

        if (@event.type == EventType.Repaint)
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

        if (mouseIsOverRect && @event is { type: EventType.MouseUp, button: 0, modifiers: EventModifiers.Control })
        {
            ToggleRowPin(index);
            GUIUtils.ReleaseMouseControl();
            //@event.Use();
        }
    }

    //private void DrawColumnsTab(ref Rect rect)
    //{
    //    var columnsTabWidgetSize = ColumnsTabWidget.GetSize(rect.size);
    //    var columnsTabRect = rect.CutByX(columnsTabWidgetSize.x + GenUI.ScrollBarWidth);
    //    var columnsTabRectMax = new Rect(Vector2.zero, columnsTabWidgetSize);
    //    // Adds empty space for more convenient vertical scrolling.
    //    columnsTabRectMax.height += columnsTabRect.height;

    //    Verse.Widgets.BeginScrollView(columnsTabRect, ref ColumnsTabScrollPosition, columnsTabRectMax, true);
    //    ColumnsTabWidget.DrawIn(columnsTabRectMax);
    //    Verse.Widgets.EndScrollView();
    //    Widgets.Draw.VerticalLine(
    //        columnsTabRect.xMax,
    //        rect.y,
    //        rect.height,
    //        MainTabWindowWidget.BorderLineColor
    //    );
    //    rect.xMin += 1f;
    //}

    private void DoHorScrollControl(Rect rect)
    {
        Event @event = Event.current;

        if (@event is { type: EventType.MouseDown, button: 0, modifiers: EventModifiers.None } && Mouse.IsOver(rect))
        {
            _rightPartIsPanned = true;
        }
        else if (_rightPartIsPanned)
        {
            if (OriginalEventUtility.EventType == EventType.MouseDrag)
            {
                _scrollPosition.x = Mathf.Max(_scrollPosition.x - @event.delta.x, 0f);
                @event.Use();
            }
            else if (@event.rawType == EventType.MouseUp)
            {
                _rightPartIsPanned = false;
                GUIUtils.ReleaseMouseControl();
                //@event.Use();
            }
        }

        // This button is here to capture control from whatever
        // eats the events above horizontal scroll code.
        rect.DrawButtonEmpty();
    }
}
