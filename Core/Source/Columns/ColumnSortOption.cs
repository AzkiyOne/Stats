using System;

namespace Stats.Columns;

public abstract class ColumnSortOption
{
    protected ColumnSortOption(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public abstract int Compare(int i1, int i2);
}

public class ColumnSortOption<T> : ColumnSortOption where T : IComparable
{
    private readonly Func<int, T> _getValue;

    public ColumnSortOption(string name, Func<int, T> getValue) : base(name)
    {
        _getValue = getValue;
    }

    public override int Compare(int i1, int i2)
    {
        return _getValue(i1).CompareTo(_getValue(i2));
    }
}
