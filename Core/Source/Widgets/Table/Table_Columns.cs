using Stats.Columns;
using Stats.Extensions;
using Stats.GUIScopes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using static Stats.GUIStyles.Table;

namespace Stats.Widgets;

public sealed partial class Table
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

    private void MoveColumn(ColumnWidget col1, ColumnWidget col2, bool placeBefore)
    {
        int col1Index = _columns.IndexOf(col1);
        int col2Index = _columns.IndexOf(col2);
        int leftColumnsCount = _leftColumnsCount;
        bool col1IsPinned = col1Index < leftColumnsCount;
        bool col2IsPinned = col2Index < leftColumnsCount;

        if (col1IsPinned && col2IsPinned == false)
        {
            _leftColumnsCount--;
        }
        else if (col1IsPinned == false && col2IsPinned)
        {
            _leftColumnsCount++;
        }

        if (placeBefore)
        {
            _columns.MoveBeforeElemAt(col1Index, col2Index);
        }
        else
        {
            _columns.MoveAfterElemAt(col1Index, col2Index);
        }
    }

    private void RefreshQueuedColum(ColumnWidget column)
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
        }

        if (_columnsToRefresh.Count == 0)
        {
            ResetColumnsToRefreshQueue();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ResetColumnsToRefreshQueue()
    {
        foreach (ColumnWidget column in _columns)
        {
            if (column.AutoRefresh)
            {
                _columnsToRefresh.Enqueue(column);
            }
        }
    }

    private sealed class ColumnWidget
    {
        private readonly Column _body;
        private readonly ColumnContentAlignment _contentAlignment;
        private readonly string _label;
        private readonly Widget? _labelShort;
        private readonly Widget _labelWidget;
        private readonly string _description;
        private readonly float _headerCellWidth;
        private readonly TipSignal _tooltip;
        private readonly FloatMenu _menu;
        private bool _isManuallyResized;
        private bool _isHidden;
        private float _width;

        public ColumnWidget(Column column, bool initialVisibility)
        {
            _body = column;
            _contentAlignment = _body.ContentAlignment;
            _isHidden = !initialVisibility;
            _label = column.Label;
            _labelShort = column.LabelShort;
            _labelWidget = column.LabelShort ?? new Label(column.Label);
            _description = column.Description;
            _headerCellWidth = _labelWidget.Size.x + GUIStyles.TableCell.PadHor;
            _tooltip = $"<i>{column.Label}</i>\n\n{column.Description}";
            _menu = new FloatMenu([
                new FloatMenuOption("Pin", () => OnPin?.Invoke(this)),
                new FloatMenuOption("Unpin", () => OnUnpin?.Invoke(this)),
                new FloatMenuOption("Hide", () => IsHidden = true)
            ]);
        }

        public event Action<ColumnWidget>? OnPin;

        public event Action<ColumnWidget>? OnUnpin;

        public event Action<ColumnWidget>? OnShow;

        public event Action<ColumnWidget>? OnHide;

        public string Label => _label;

        public Widget? LabelShort => _labelShort;

        public string Description => _description;

        public float Width => _width;

        public bool AutoRefresh => _body.AutoRefresh;

        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                if (_isHidden != value)
                {
                    _isHidden = value;

                    if (value == true)
                    {
                        OnHide?.Invoke(this);
                    }
                    else
                    {
                        OnShow?.Invoke(this);
                    }
                }
            }
        }

        public ICollection<ColumnFilterOption> FilterOptions => _body.FilterOptions;

        public void Draw(Rect rect, GUIListSegment<int> topRows, GUIListSegment<int> bottomRows)
        {
            float topRowsHeight = topRows.Length * RowHeight;

            rect.CutTop(out Rect headerCellRect, HeadersRowHeight)
                .CutTop(out Rect topRowsRect, topRowsHeight)
                .TakeRest(out Rect bottomRowsRect);

            DrawHeader(headerCellRect);

            // Do we actually need to draw cells on Layout event?
            // Normally we would, so they can refresh themselves.
            // But given how column's cells are refreshed, there is no point in doing this.
            if (topRows.Length > 0)
            {
                using (new GUIClipScope(topRowsRect))
                {
                    DrawBody(topRowsRect with { x = 0f, y = 0f }, topRows);
                }
            }

            if (bottomRows.Length > 0)
            {
                using (new GUIClipScope(bottomRowsRect))
                {
                    DrawBody(bottomRowsRect with { x = 0f, y = bottomRows.AxisStart }, bottomRows);
                }
            }

            if (_isManuallyResized == false && Event.current.type == EventType.Layout)
            {
                float maxBodyCellWidth = 0f;

                try
                {
                    maxBodyCellWidth = Mathf.Max(_body.GetMaxCellWidth(topRows.List), _body.GetMaxCellWidth(bottomRows.List));
                }
                catch (Exception e)
                {
                    HandleLayoutRefreshException(e);
                }

                _width = Mathf.Max(_headerCellWidth, maxBodyCellWidth);
            }
        }

        private void DrawBody(Rect rect, GUIListSegment<int> rows)
        {
            Rect cellRect = rect with { height = RowHeight };

            for (int i = rows.Start; i < rows.End; i++)
            {
                try
                {
                    _body.DrawCell(cellRect, rows.List[i]);
                }
                catch (Exception e)
                {
                    DrawFaultyBodyCell(cellRect, e);
                }

                cellRect.y = cellRect.yMax;
            }
        }

        private void DrawHeader(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Rect labelRect = rect.ContractedBy(GUIStyles.TableCell.PadLR, GUIStyles.TableCell.PadTB);

                using (new GUIClipScopeSimple(labelRect))
                {
                    labelRect = labelRect with { x = 0f, y = 0f };
                    float labelWidgetWidth = _labelWidget.Size.x;

                    labelRect = _contentAlignment switch
                    {
                        ColumnContentAlignment.Right => labelRect.RightPartPixels(labelWidgetWidth),
                        ColumnContentAlignment.Middle => labelRect.MiddlePartPixels(labelWidgetWidth, labelRect.height),
                        _ => labelRect.LeftPartPixels(labelWidgetWidth),
                    };

                    _labelWidget.Draw(labelRect);
                }

                rect.DrawButtonGhostlySkin()
                    .DrawBorderRight(ColumnSeparatorLineColor);
            }

            rect.CutRight(out Rect resizeControlRect, GUIStyles.TableCell.PadLR - 1f)
                .TakeRest(out Rect labelControlRect);

            DoLabelControl(labelControlRect);
            DoResizeControl(resizeControlRect);
        }

        private void DoLabelControl(Rect rect)
        {
            Event @event = Event.current;

            rect.DoDraggableControl(this);

            if (@event is { type: EventType.MouseUp, button: 1 } && Mouse.IsOver(rect))
            {
                _menu.Open();
                @event.Use();
            }

            rect.Tip(_tooltip);
        }

        private readonly object _resizeWidgetToken = new();

        private void DoResizeControl(Rect rect)
        {
            Event curEvent = Event.current;
            GUIUtils.DragState dragState = rect.DoDraggableControl(_resizeWidgetToken);

            if (dragState == GUIUtils.DragState.DragStarted)
            {
                _isManuallyResized = true;
            }
            else if (dragState == GUIUtils.DragState.Dragged && curEvent.type == EventType.Layout)
            {
                _width = Mathf.Clamp(_width + GUIUtils.MouseDelta.x, HeadersRowHeight, float.MaxValue);
            }
            else if (curEvent is { type: EventType.MouseDown, clickCount: > 1 } && Mouse.IsOver(rect))
            {
                _isManuallyResized = false;
                curEvent.Use();
            }

            if (curEvent.type == EventType.Repaint)
            {
                if (dragState == GUIUtils.DragState.Dragged)
                {
                    rect.HighlightActive();
                }
                else
                {
                    rect.DrawButtonGhostlySkinSilent();
                }
            }
        }

        public void RefreshCells()
        {
            _body.RefreshCells();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandleLayoutRefreshException(Exception e)
        {
            Log.Error($"Unable to update width of \"{_label}\" column. Switching to manual mode to prevent further errors: {e.Message}");

            _isManuallyResized = true;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void DrawFaultyBodyCell(Rect rect, Exception e)
        {
            rect.Fill(Color.red);

            if (Mouse.IsOver(rect))
            {
                rect.Tip(e.Message);
            }
        }
    }
}
