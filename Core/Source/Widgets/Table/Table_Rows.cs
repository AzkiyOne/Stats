using System.Runtime.CompilerServices;
using Stats.Extensions;

namespace Stats.Widgets;

public sealed partial class Table<TRecord>
{
    private void SwapRecords(int i1, int i2)
    {
        _records.Swap(i1, i2);

        foreach (ColumnWidget column in _columns)
        {
            column.Swap(i1, i2);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ToggleRowPin(int index)
    {
        _beforeDraw = () =>
        {
            if (index < _topRowsCount)
            {
                SwapRecords(_rows[index], _topRowsCount - 1);

                _topRowsCount--;
            }
            else
            {
                SwapRecords(_rows[index], _topRowsCount);

                _topRowsCount++;
            }

            SyncTopRowsWithRecords();
            ClearBottomRows();

            _doFilter = true;
        };
    }

    private void SyncTopRowsWithRecords()
    {
        for (int i = 0; i < _topRowsCount; i++)
        {
            _rows[i] = i;
        }
    }

    private void ClearBottomRows()
    {
        _rows.RemoveRange(_topRowsCount, BottomRowsCount);
    }
}
