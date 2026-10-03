namespace Stats.NumberFormats;

public class Standard : NumberFormat
{
    public Standard(NumberFormatProps props) : this(props.pattern)
    {
    }

    public Standard(string pattern = "0") : base(pattern)
    {
        FormatString = pattern;
    }

    protected override string FormatString { get; }
}
