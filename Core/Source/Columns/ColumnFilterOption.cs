using System;
using System.Collections.Generic;
using Stats.Widgets.Filters;

namespace Stats.Columns;

public abstract class ColumnFilterOption
{
    protected ColumnFilterOption(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public abstract Filter GetFilter();
}

public class NumberColumnFilterOption : ColumnFilterOption
{
    private readonly Func<int, decimal> _getValue;

    public NumberColumnFilterOption(string name, Func<int, decimal> getValue) : base(name)
    {
        _getValue = getValue;
    }

    public override Filter GetFilter()
    {
        return new NumberFilter(_getValue);
    }
}

public class BooleanColumnFilterOption : ColumnFilterOption
{
    private readonly Func<int, bool> _getValue;

    public BooleanColumnFilterOption(string name, Func<int, bool> getValue) : base(name)
    {
        _getValue = getValue;
    }

    public override Filter GetFilter()
    {
        return new BooleanFilter(_getValue);
    }
}

public class StringColumnFilterOption : ColumnFilterOption
{
    private readonly Func<int, string> _getValue;

    public StringColumnFilterOption(string name, Func<int, string> getValue) : base(name)
    {
        _getValue = getValue;
    }

    public override Filter GetFilter()
    {
        return new StringFilter(_getValue);
    }
}

public class OTMColumnFilterOption<T> : ColumnFilterOption
{
    private readonly Func<int, T> _getValue;
    private readonly IEnumerable<NTMFilterOption<T>> _options;

    public OTMColumnFilterOption(string name, Func<int, T> getValue, IEnumerable<NTMFilterOption<T>> options) : base(name)
    {
        _getValue = getValue;
        _options = options;
    }

    public override Filter GetFilter()
    {
        return new OTMFilter<T>(_getValue, _options);
    }
}

public class MTMColumnFilterOption<T> : ColumnFilterOption
{
    private readonly Func<int, IEnumerable<T>> _getValue;
    private readonly IEnumerable<NTMFilterOption<T>> _options;

    public MTMColumnFilterOption(string name, Func<int, IEnumerable<T>> getValue, IEnumerable<NTMFilterOption<T>> options) : base(name)
    {
        _getValue = getValue;
        _options = options;
    }

    public override Filter GetFilter()
    {
        return new MTMFilter<T>(_getValue, _options);
    }
}
