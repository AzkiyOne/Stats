using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stats.Widgets;

public sealed partial class Table
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ToggleRowPin(int index, bool isPinned)
    {
        if (isPinned)
        {
            int recordId = _topRows[index];

            if (FilterRecord(recordId))
            {
                _bottomRows.Add(recordId);

                _sortWidget.SortRows(_bottomRows);
            }

            _topRows.RemoveAt(index);
        }
        else
        {
            _topRows.Add(_bottomRows[index]);
            _bottomRows.RemoveAt(index);

            _sortWidget.SortRows(_topRows);
        }
    }

    internal readonly struct GUIListSegment<T>
    {
        private readonly List<T> _list;
        private readonly int _start;
        private readonly int _length;
        private readonly int _end;
        private readonly float _axisStart;

        public GUIListSegment(List<T> list) : this(list, 0, list.Count, 0f)
        {
        }

        public GUIListSegment(List<T> list, int start, int length, float axisStart)
        {
            _list = list;
            _start = start;
            _length = length;
            _end = start + length;
            _axisStart = axisStart;
        }

        public List<T> List => _list;

        public int Start => _start;

        public int Length => _length;

        /// <remarks>Exclusive</remarks>
        public int End => _end;

        public float AxisStart => _axisStart;
    }
}
