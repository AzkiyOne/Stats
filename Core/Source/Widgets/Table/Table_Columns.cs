using System;
using System.Collections.Generic;
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

    private sealed class ColumnWidget
    {
        private readonly Column<TRecord> _column;
        private readonly ColumnContentAlignment _contentAlignment;
        private readonly Widget _labelWidget;
        private readonly TipSignal _tooltip;
        private readonly FloatMenu _menu;
        private bool _isResized;
        private bool _isManuallyResized;

        public ColumnWidget(Column<TRecord> column)
        {
            _column = column;
            _contentAlignment = _column.ContentAlignment;
            ColumnDef columnDef = column.Def;
            _labelWidget = columnDef.LabelWidget;
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
        }

        public event Action<ColumnWidget>? OnPin;

        public event Action<ColumnWidget>? OnUnpin;

        public event Action<ColumnWidget>? OnShow;

        public event Action<ColumnWidget>? OnHide;

        public float Width { get; private set; }

        public bool IsRefreshable => _column.IsRefreshable;

        public bool IsHidden
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;

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
            }
        }

        public ColumnDef Def => _column.Def;

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
                // TODO: 
                // - Make columns return cell width with padding. Reason - consistency with DrawCell methods.
                // - Make PadHor/Ver be a sum of left + right/top + bottom padding.
                // - Introduce PadLR/PadTB.
                Width = Mathf.Max(_labelWidget.Size.x, _column.GetMinWidth(rows)) + GUIStyles.TableCell.PadHor * 2f;
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
                catch
                {
                    // TODO:
                    // - Add tooltip with exception's message.
                    // - Make the whole thing into a separate non-inlineable method.
                    cellRect.Fill(Color.red);
                }

                cellRect.y = cellRect.yMax;
            }
        }

        private void DrawHeaderCell(Rect rect, DragManager<ColumnWidget> dragManager)
        {
            Event @event = Event.current;
            ColumnContentAlignment contentAlignment = _contentAlignment;
            const float SideControlMargin = 1f;
            rect.CutLeft(out Rect sortControlRect, GUIStyles.TableCell.PadHor - SideControlMargin)
                .CutRight(out Rect resizeControlRect, GUIStyles.TableCell.PadHor - SideControlMargin)
                .TakeRest(out Rect labelControlRect);

            if (@event.type == EventType.Repaint)
            {
                Rect labelClipRect = labelControlRect.ContractedBy(SideControlMargin, GUIStyles.TableCell.PadVer);
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

        public void Unfocus()
        {
            if (_isResized)
            {
                _isResized = false;
            }
        }

        public void AddRecord(TRecord record)
        {
            _column.AddRecord(record);
        }

        public void RemoveRecord(int i)
        {
            _column.RemoveRecord(i);
        }

        public void RefreshCells()
        {
            _column.RefreshCells();
        }
    }
}
