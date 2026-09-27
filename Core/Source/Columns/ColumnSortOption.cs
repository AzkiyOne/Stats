using System;
using System.Collections.Generic;

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

public class ColumnSortOption<T> : ColumnSortOption where T : IComparable<T>
{
    private readonly List<T> _values;

    public ColumnSortOption(string name, List<T> values) : base(name)
    {
        _values = values;
    }

    public override int Compare(int i1, int i2)
    {
        return _values[i1].CompareTo(_values[i2]);
    }
}
