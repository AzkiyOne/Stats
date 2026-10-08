using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Columns;
using Stats.Extensions;
using Stats.Widgets.Filters;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed partial class Table<TRecord>
{
    private void ApplyFilters()
    {
        List<Filter> filters = _filtersTab.ActiveFilters;
        int filtersCount = filters.Count;

        if (filtersCount > 0)
        {
            ClearBottomRows();

            bool mode = _filtersTab.Mode;

            for (int r = _topRowsCount; r < _records.Count; r++)
            {
                bool recordFitsQuery = !mode;

                for (int f = 0; f < filtersCount; f++)
                {
                    Filter filter = filters[f];

                    if (filter.Eval(r) == mode)
                    {
                        recordFitsQuery = mode;

                        break;
                    }
                }

                if (recordFitsQuery)
                {
                    _rows.Add(r);
                }
            }
        }
        else if (_rows.Count != _records.Count)
        {
            ClearBottomRows();

            for (int i = _topRowsCount; i < _records.Count; i++)
            {
                _rows.Add(i);
            }
        }
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

            foreach (FilterListItem filter in _filters)
            {
                Vector2 filterSize = filter.Size;
                rect = rect.CutTop(out Rect filterRect, filterSize.y);

                filter.Draw(filterRect);
            }

            if (eventType == EventType.Layout)
            {
                float width = 300f;

                if (_filters.Count > 0)
                {
                    width = Mathf.Max(width, _filters.Select(filter => filter.Size.x).Max());
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
