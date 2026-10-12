using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Columns;
using Stats.Extensions;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed partial class Table
{
    private sealed class SortWidget
    {
        private const int SortDirectionAscending = 1;
        private const int SortDirectionDescending = -1;

        private static readonly GUIStyle _currentSortOptionLabelStyle = new(GUIStyles.TableToolbarButton.LabelStyle)
        {
            padding = new RectOffset((int)GUIStyles.TableToolbarButton.PadLR,
                                     (int)GUIStyles.TableToolbarButton.PadLR,
                                     (int)GUIStyles.TableToolbarButton.PadTB,
                                     (int)GUIStyles.TableToolbarButton.PadTB)
        };

        private ColumnSortOption _currentSortOption;
        private int _sortDirection = SortDirectionAscending;
        private float _width;
        private readonly FloatMenu _menu;

        public SortWidget(IEnumerable<ColumnSortOption> sortOptions)
        {
            _currentSortOption = sortOptions.First();
            _width = _currentSortOption.Name.CalcSize(_currentSortOptionLabelStyle).x;

            List<FloatMenuOption> menuOptions = new(10);

            foreach (ColumnSortOption sortOption in sortOptions)
            {
                string label = sortOption.Name;

                FloatMenuOption menuOption = new(label, () =>
                {
                    _currentSortOption = sortOption;
                    _width = label.CalcSize(_currentSortOptionLabelStyle).x;

                    OnChange?.Invoke();
                });

                menuOptions.Add(menuOption);
            }

            _menu = new FloatMenu(menuOptions);
        }

        public event Action? OnChange;

        public float Width => _width;

        public void Draw(Rect rect)
        {
            string label = _currentSortOption.Name;

            label.Draw(rect, _currentSortOptionLabelStyle);

            if (rect.DrawButtonGhostly())
            {
                _menu.Open();
            }
        }

        public void SortRows(List<int> rows)
        {
            rows.Sort((r1, r2) =>
            {
                int result = _currentSortOption.Compare(r1, r2);

                // Stable sort
                if (result == 0)
                {
                    result = r1.CompareTo(r2);
                }

                return result * _sortDirection;
            });
        }
    }
}
