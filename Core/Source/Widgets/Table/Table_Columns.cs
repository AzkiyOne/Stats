using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.Columns;
using Stats.Extensions;
using Stats.GUIScopes;
using UnityEngine;
using Verse;
using Verse.Sound;
using static Stats.GUIStyles.Table;

namespace Stats.Widgets;

public sealed partial class Table<TRecord>
{
    private void PinColumn(ColumnWidget column)
    {
        int columnIndex = _columns.IndexOf(column);

        if (columnIndex != _leftColumnsCount)
        {
            int lastPinnedColumnIndex = _leftColumnsCount - 1;
            _columns.MoveAfterElemAt(columnIndex, lastPinnedColumnIndex);
        }

        _leftColumnsCount++;
    }

    private void UnpinColumn(ColumnWidget column)
    {
        int columnIndex = _columns.IndexOf(column);
        int lastPinnedColumnIndex = _leftColumnsCount - 1;

        if (columnIndex != lastPinnedColumnIndex)
        {
            _columns.MoveAfterElemAt(columnIndex, lastPinnedColumnIndex);
        }

        _leftColumnsCount--;
    }

    private void HandleColumnDrag(ColumnWidget draggedColumn, ColumnWidget column, bool placeBefore)
    {
        int draggedColumnIndex = _columns.IndexOf(draggedColumn);
        int columnIndex = _columns.IndexOf(column);
        int leftColumnsCount = _leftColumnsCount;
        bool draggedColumnIsPinned = draggedColumnIndex < leftColumnsCount;
        bool columnIsPinned = columnIndex < leftColumnsCount;

        if (draggedColumnIsPinned && columnIsPinned == false)
        {
            _leftColumnsCount--;
        }
        else if (draggedColumnIsPinned == false && columnIsPinned)
        {
            _leftColumnsCount++;
        }

        if (placeBefore)
        {
            _columns.MoveBeforeElemAt(draggedColumnIndex, columnIndex);
        }
        else
        {
            _columns.MoveAfterElemAt(draggedColumnIndex, columnIndex);
        }
    }

    private void RefreshColumnOnce(ColumnWidget columnWidget)
    {
        // Manually refreshed columns are queued only after we finish the current queue.
        // Until that, we'll put them into a separate collection where they will wait to be queued.
        // This is done in order to prevent them from locking the queue by spamming RefreshOnce.
        _columnsToQueueOnNextCycle.Add(columnWidget);
    }

    private void RefreshQueuedColum(ColumnWidget column)
    {
        // TODO: Implementation
        //bool columnWasAltered = false;
        bool columnWasAltered = true;

        try
        {
            column.RefreshCells(_records);
        }
        catch (Exception e)
        {
            LogUnableToRefreshColumn(e, column);
        }

        if (columnWasAltered)
        {
            _doFilter = true;
        }

        if (_columnsToRefresh.Count == 0)
        {
            ResetAutoRefreshableColumnsQueue();
        }
    }

    private void ResetAutoRefreshableColumnsQueue()
    {
        foreach (ColumnWidget column in _columns)
        {
            if (column.AutoRefresh)
            {
                _columnsToRefresh.Enqueue(column);
            }
        }

        foreach (ColumnWidget column in _columnsToQueueOnNextCycle)
        {
            // Normally, auto refreshable columns do not manually refresh themselves, but who knows.
            if (_columnsToRefresh.Contains(column) == false)
            {
                _columnsToRefresh.Enqueue(column);
            }
        }

        _columnsToQueueOnNextCycle.Clear();
    }

    private sealed class ColumnWidget
    {
        private readonly Column<TRecord> _column;
        private readonly ColumnContentAlignment _contentAlignment;
        private readonly Widget _labelWidget;
        private readonly float _headerCellWidth;
        private readonly TipSignal _tooltip;
        private readonly FloatMenu _menu;
        private bool _isResized;
        private bool _isManuallyResized;
        private bool _isHidden;

        public ColumnWidget(Column<TRecord> column, bool initialVisibility)
        {
            _column = column;
            _contentAlignment = _column.ContentAlignment;
            _isHidden = !initialVisibility;
            ColumnDef columnDef = column.Def;
            _labelWidget = columnDef.LabelWidget;
            _headerCellWidth = _labelWidget.Size.x + GUIStyles.TableCell.PadHor;
            _tooltip = $"<i>{columnDef.LabelCap}</i>\n\n{columnDef.description}";
            _menu = new FloatMenu([
                //new FloatMenuOption("Sort Asc", () => {
                //    // TODO
                //}, TexButton.ReorderUp, Color.white),
                //new FloatMenuOption("Sort Desc", () => {
                //    // TODO
                //}, TexButton.ReorderDown, Color.white),
                new FloatMenuOption("Pin", () => OnPin?.Invoke(this)),
                new FloatMenuOption("Unpin", () => OnUnpin?.Invoke(this)),
                new FloatMenuOption("Hide", () => IsHidden = true, TexButton.Suspend, Color.white)
            ]);

            column.OnRefreshOnce += () => OnRefreshOnce?.Invoke(this);
        }

        public event Action<ColumnWidget>? OnPin;

        public event Action<ColumnWidget>? OnUnpin;

        public event Action<ColumnWidget>? OnShow;

        public event Action<ColumnWidget>? OnHide;

        public event Action<ColumnWidget>? OnRefreshOnce;

        public float Width { get; private set; }

        public bool AutoRefresh => _column.AutoRefresh;

        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                if (_isHidden != value)
                {
                    _isHidden = value;

                    try
                    {
                        if (value == true)
                        {
                            _column.Hide();
                            OnHide?.Invoke(this);
                        }
                        else
                        {
                            _column.Show();
                            OnShow?.Invoke(this);
                        }
                    }
                    catch (Exception e)
                    {
                        _isHidden = !_isHidden;

                        LogUnableToChangeVisibility(e);
                    }
                }
            }
        }

        public ColumnDef Def => _column.Def;

        public ICollection<ColumnFilterOption> FilterOptions => _column.FilterOptions;

        public void Draw(Rect rect, List<int> rows, Span<int> topRows, Span<int> visibleBottomRows, float bottomRowsY, DragManager<ColumnWidget> dragManager)
        {
            float topRowsHeight = topRows.Length * RowHeight;

            rect.CutTop(out Rect headerCellRect, HeadersRowHeight)
                .CutTop(out Rect topRowsRect, topRowsHeight)
                .TakeRest(out Rect bottomRowsRect);

            DrawHeaderCell(headerCellRect, dragManager);

            // Do we actually need to draw cells on Layout event?
            // Normally we would, so they can refresh themselves.
            // But given how column's cells are refreshed, there is no point in doing this.
            if (topRows.Length > 0)
            {
                using (new GUIClipScope(topRowsRect))
                {
                    DrawCells(topRowsRect with { x = 0f, y = 0f }, topRows);
                }
            }

            if (visibleBottomRows.Length > 0)
            {
                using (new GUIClipScope(bottomRowsRect, new Vector2(0f, bottomRowsY)))
                {
                    DrawCells(bottomRowsRect with { x = 0f, y = 0f }, visibleBottomRows);
                }
            }

            if (Event.current.type == EventType.Layout && _isManuallyResized == false)
            {
                float maxBodyCellWidth = 0f;

                try
                {
                    maxBodyCellWidth = _column.GetMaxCellWidth(rows);
                }
                catch (Exception e)
                {
                    LogUnableToUpdateWidth(e);
                }

                Width = Mathf.Max(_headerCellWidth, maxBodyCellWidth);
            }
        }

        private void DrawCells(Rect rect, Span<int> rows)
        {
            Rect cellRect = rect with { height = RowHeight };
            int rowsCount = rows.Length;

            for (int i = 0; i < rowsCount; i++)
            {
                try
                {
                    _column.DrawCell(cellRect, rows[i]);
                }
                catch (Exception e)
                {
                    DrawFaultyCell(cellRect, e);
                }

                cellRect.y = cellRect.yMax;
            }
        }

        private void DrawHeaderCell(Rect rect, DragManager<ColumnWidget> dragManager)
        {
            Event @event = Event.current;
            ColumnContentAlignment contentAlignment = _contentAlignment;
            const float SideControlMargin = 1f;
            rect.CutLeft(out Rect sortControlRect, GUIStyles.TableCell.PadLR - SideControlMargin)
                .CutRight(out Rect resizeControlRect, GUIStyles.TableCell.PadLR - SideControlMargin)
                .TakeRest(out Rect labelControlRect);

            if (@event.type == EventType.Repaint)
            {
                Rect labelClipRect = labelControlRect.ContractedBy(SideControlMargin, GUIStyles.TableCell.PadTB);
                GUI.BeginClip(labelClipRect);

                float labelWidgetWidth = _labelWidget.Size.x;
                Rect labelRect = labelClipRect with { x = 0f, y = 0f };
                if (contentAlignment == ColumnContentAlignment.Right)
                {
                    labelRect.CutRight(out labelRect, labelWidgetWidth);
                }
                else if (contentAlignment == ColumnContentAlignment.Middle)
                {
                    labelRect.CutMidX(out labelRect, labelWidgetWidth);
                }
                else
                {
                    labelRect = labelRect with { width = labelWidgetWidth };
                }
                _labelWidget.Draw(labelRect);

                GUI.EndClip();

                if (dragManager.IsDragged(this))
                {
                    rect.HighlightActive();
                }
                else if (Mouse.IsOver(rect))
                {
                    rect.HighlightLight();
                }

                rect.DrawBorderRight(ColumnSeparatorLineColor);
            }

            MouseoverSounds.DoRegion(rect);

            DoSortControl(sortControlRect);
            dragManager.OnGUI(labelControlRect, rect, this);
            DoLabelControl(labelControlRect);
            DoResizeControl(resizeControlRect);

            labelControlRect.Tip(_tooltip);
        }

        private void DoLabelControl(Rect rect)
        {
            Event @event = Event.current;

            if (@event is { type: EventType.MouseUp, button: 1, modifiers: EventModifiers.None } && Mouse.IsOver(rect))
            {
                _menu.Open();
                GUIUtils.ReleaseMouseControl();
                //@event.Use();
            }

            rect.DrawButtonEmpty();
        }

        private void DoSortControl(Rect rect)
        {
            Event @event = Event.current;
            const float IconPadding = 3f;

            if (@event.type == EventType.Repaint)
            {
                // TODO
                //if (parent._sortColumn == this)
                //{
                //    if (parent._sortDirection == SortDirectionAscending)
                //    {
                //        rect.TopHalf()
                //            .ContractedBy(IconPadding)
                //            .DrawTextureFitted(TexButton.ReorderUp);
                //    }
                //    else
                //    {
                //        rect.BottomHalf()
                //            .ContractedBy(IconPadding)
                //            .DrawTextureFitted(TexButton.ReorderDown);
                //    }
                //}

                if (Mouse.IsOver(rect))
                {
                    rect.Highlight();
                }
            }

            bool wasClicked = rect.DrawButtonEmpty();
            if (wasClicked && @event is { button: 0, modifiers: EventModifiers.None })
            {
                // TODO
                //if (parent._sortColumn != this)
                //{
                //    parent._sortColumn = this;
                //}
                //else
                //{
                //    parent._sortDirection *= -1;
                //}
            }
        }

        private void DoResizeControl(Rect rect)
        {
            Event @event = Event.current;
            bool mouseIsOverRect = Mouse.IsOver(rect);

            if (@event is { type: EventType.MouseDown, button: 0, modifiers: EventModifiers.None } && mouseIsOverRect)
            {
                if (@event.clickCount > 1)
                {
                    _isManuallyResized = false;
                }
                else
                {
                    _isResized = true;
                    _isManuallyResized = true;
                }
            }
            else if (_isResized)
            {
                if (OriginalEventUtility.EventType == EventType.MouseDrag)
                {
                    Width = Mathf.Clamp(Width + @event.delta.x, HeadersRowHeight, float.MaxValue);
                    @event.Use();
                }
                // TODO: This will not work if the column will stop being rendered as the result of resizing.
                else if (@event.rawType == EventType.MouseUp)
                {
                    _isResized = false;
                    GUIUtils.ReleaseMouseControl();
                    //@event.Use();
                }
            }

            if (@event.type == EventType.Repaint && (mouseIsOverRect || _isResized))
            {
                rect.HighlightActive();
            }

            //GUI.SetNextControlName($"{Def.defName}_ColumnResizeControl");
            rect.DrawButtonEmpty();
        }

        public void Suspend()
        {
            if (_isResized)
            {
                _isResized = false;
            }
        }

        public void Add(TRecord record)
        {
            _column.Add(record);
        }

        public void Swap(int i1, int i2)
        {
            _column.Swap(i1, i2);
        }

        public void Replace(int i1, int i2)
        {
            _column.Replace(i1, i2);
        }

        public void RefreshCells(List<TRecord> records)
        {
            for (int i = 0; i < records.Count; i++)
            {
                TRecord record = records[i];

                _column.Refresh(i, record);
            }
        }

        public void Dispose()
        {
            _column.Dispose();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void LogUnableToChangeVisibility(Exception e)
        {
            Log.Error($"Unable to change visibility of \"{_column.Def.defName}\" column: {e.Message}");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void LogUnableToUpdateWidth(Exception e)
        {
            Log.Error($"Unable to update width of \"{_column.Def.defName}\" column: {e.Message}");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void DrawFaultyCell(Rect rect, Exception e)
        {
            rect.Fill(Color.red);

            if (Mouse.IsOver(rect))
            {
                rect.Tip(e.Message);
            }
        }
    }
}
