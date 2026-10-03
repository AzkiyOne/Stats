namespace Stats.NumberFormats;

public sealed class Percent : Standard
{
    public Percent(NumberFormatProps props) : this(props.pattern)
    {
    }

    public Percent(string pattern) : base(pattern.Contains('%') ? pattern : pattern + "\\%")
    {
    }

    public override decimal ToDecimal(float value)
    {
        return base.ToDecimal(value * 100f);
    }
}
