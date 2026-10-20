using Stats.Columns;
using Stats.Extensions;
using Stats.Widgets.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed partial class Table
{
    private void ApplyFilters()
    {
        List<Filter> filters = _filtersTab.ActiveFilters;
        int filtersCount = filters.Count;

        if (filtersCount > 0)
        {
            _bottomRows.Clear();

            bool mode = _filtersTab.Mode;
            Span<int> topRows = stackalloc int[_topRows.Count];
            _topRows.CopyTo(topRows);

            for (int r = 0; r < _recordsCount; r++)
            {
                if (topRows.IndexOf(r) < 0)
                {
                    bool recordFitsQuery = FilterRecord(r, filters, mode);

                    if (recordFitsQuery)
                    {
                        _bottomRows.Add(r);
                    }
                }
            }
        }
        else if (RowsCount != _recordsCount)
        {
            _bottomRows.Clear();

            Span<int> topRows = stackalloc int[_topRows.Count];
            _topRows.CopyTo(topRows);

            for (int i = 0; i < _recordsCount; i++)
            {
                if (topRows.IndexOf(i) < 0)
                {
                    _bottomRows.Add(i);
                }
            }
        }
    }

    private bool FilterRecord(int r)
    {
        List<Filter> filters = _filtersTab.ActiveFilters;

        if (filters.Count > 0)
        {
            return FilterRecord(r, filters, _filtersTab.Mode);
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool FilterRecord(int r, List<Filter> filters, bool mode)
    {
        bool recordFitsQuery = !mode;

        for (int f = 0; f < filters.Count; f++)
        {
            Filter filter = filters[f];

            if (filter.Eval(r) == mode)
            {
                recordFitsQuery = mode;

                break;
            }
        }

        return recordFitsQuery;
    }

    private sealed class FiltersTab
    {
        private readonly FloatMenu _filtersMenu;
        private readonly List<FilterListItem> _filters;

        public FiltersTab(IEnumerable<ColumnFilterOption> filterOptions)
        {
            List<FloatMenuOption> menuOptions = new(20);
            List<FilterListItem> filters = new(10);

            foreach (ColumnFilterOption filterOption in filterOptions)
            {
                string label = filterOption.Name;

                FloatMenuOption menuOption = new(
                    label,
                    () => AddFilter(filterOption));

                menuOptions.Add(menuOption);
            }

            _filtersMenu = new FloatMenu(menuOptions);
            _filters = filters;
            ActiveFilters = new List<Filter>(10);
        }

        public float Width { get; private set; }

        public List<Filter> ActiveFilters { get; }

        // AND - false
        // OR - true
        public bool Mode { get; private set; }

        public event Action? OnChange;

        private void AddFilter(ColumnFilterOption filterOption)
        {
            string label = filterOption.Name;
            Filter filter = filterOption.GetFilter();

            filter.OnChange += () =>
            {
                if (filter.IsActive)
                {
                    if (ActiveFilters.Contains(filter) == false)
                    {
                        ActiveFilters.Add(filter);
                    }
                }
                else
                {
                    ActiveFilters.Remove(filter);
                }

                OnChange?.Invoke();
            };

            _filters.Add(new FilterListItem(label, filter));
        }

        public void Draw(Rect rect)
        {
            EventType eventType = Event.current.type;

            rect = rect.CutTop(out Rect addButtonRect, 30f);

            bool addButtonWasClicked = addButtonRect.DrawButtonSubtle("+ Add Filter");

            if (addButtonWasClicked)
            {
                _filtersMenu.Open();
            }

            for (int i = 0; i < _filters.Count; i++)
            {
                FilterListItem filter = _filters[i];
                Vector2 filterSize = filter.Size;
                rect = rect.CutTop(out Rect filterRect, filterSize.y);

                filter.Draw(filterRect);
            }

            if (eventType == EventType.Layout)
            {
                float width = 300f;

                if (_filters.Count > 0)
                {
                    width = Mathf.Max(width, _filters.Max(filter => filter.Size.x));
                }

                Width = width;
            }
        }

        private sealed class FilterListItem
        {
            private static readonly GUIStyle _labelStyle = new(GUIStyles.Text.FontMedium);

            private readonly string _label;
            private readonly Vector2 _labelSize;
            private readonly Filter _filter;

            public FilterListItem(string label, Filter filter)
            {
                _label = label;
                _labelSize = label.CalcSize(_labelStyle);
                _filter = filter;
            }

            public Vector2 Size { get; private set; }

            public void Draw(Rect rect)
            {
                EventType eventType = Event.current.type;

                rect.CutLeft(out Rect labelRect, _labelSize)
                    .TakeRest(out Rect filterRect);

                _label.Draw(labelRect, _labelStyle);
                _filter.Draw(filterRect, Vector2.zero);

                if (eventType == EventType.Layout)
                {
                    Vector2 size;
                    size.x = _labelSize.x + _filter.GetSize().x;
                    size.y = Mathf.Max(_labelSize.y, _filter.GetSize().y);

                    Size = size;
                }
            }
        }
    }
}
