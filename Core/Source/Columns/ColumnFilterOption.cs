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
    private readonly List<decimal> _values;

    public NumberColumnFilterOption(string name, List<decimal> values) : base(name)
    {
        _values = values;
    }

    public override Filter GetFilter()
    {
        return new NumberFilter(_values);
    }
}

public class BooleanColumnFilterOption : ColumnFilterOption
{
    private readonly List<bool> _values;

    public BooleanColumnFilterOption(string name, List<bool> values) : base(name)
    {
        _values = values;
    }

    public override Filter GetFilter()
    {
        return new BooleanFilter(_values);
    }
}

public class StringColumnFilterOption : ColumnFilterOption
{
    private readonly List<string> _values;

    public StringColumnFilterOption(string name, List<string> values) : base(name)
    {
        _values = values;
    }

    public override Filter GetFilter()
    {
        return new StringFilter(_values);
    }
}

public class OTMColumnFilterOption<T> : ColumnFilterOption
{
    private readonly List<T> _values;
    private readonly IEnumerable<NTMFilterOption<T>> _options;

    public OTMColumnFilterOption(string name, List<T> values, IEnumerable<NTMFilterOption<T>> options) : base(name)
    {
        _values = values;
        _options = options;
    }

    public override Filter GetFilter()
    {
        return new OTMFilter<T>(_values, _options);
    }
}

public class MTMColumnFilterOption<T> : ColumnFilterOption
{
    private readonly List<IEnumerable<T>> _values;
    private readonly IEnumerable<NTMFilterOption<T>> _options;

    public MTMColumnFilterOption(string name, List<IEnumerable<T>> values, IEnumerable<NTMFilterOption<T>> options) : base(name)
    {
        _values = values;
        _options = options;
    }

    public override Filter GetFilter()
    {
        return new MTMFilter<T>(_values, _options);
    }
}
