using Verse;

namespace Stats.NumberFormats;

public sealed class Money : Standard
{
    public Money(NumberFormatProps props) : this(props.pattern)
    {
    }

    public Money(string pattern) : base("MoneyFormat".Translate(pattern))
    {
    }
}
