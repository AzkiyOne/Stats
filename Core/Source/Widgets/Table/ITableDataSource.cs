using System;

namespace Stats.Widgets;

public interface ITableDataSource
{
    public int RecordsCount { get; }

    public event Action OnAdd;

    public event Action<int, int> OnReplace;
}
