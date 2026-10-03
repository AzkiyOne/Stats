using System;
using System.Linq;
using System.Text.RegularExpressions;
using Stats.Extensions;
using Verse;

namespace Stats.NumberFormats;

public abstract class NumberFormat
{
    private static readonly Regex _digitsRegex = new(@"\.(0)?", RegexOptions.Compiled);

    private readonly int _digits;

    protected NumberFormat(string pattern)
    {
        _digits = _digitsRegex.Match(pattern).Value.Length;
    }

    protected abstract string FormatString { get; }

    public virtual decimal ToDecimal(float number)
    {
        return number.ToDecimal(_digits);
    }

    public virtual string FormatNumber(decimal number)
    {
        return number.ToString(FormatString);
    }
}

public class NumberFormatProps
{
    private static readonly Regex _translationTokensRegex = new(@"\{(\w*)\}", RegexOptions.Compiled);

    public string pattern = "0";
    public Type formatClass = typeof(Standard);

    public NumberFormat NumberFormatInstance => field ??= (NumberFormat)Activator.CreateInstance(formatClass, this);

    public virtual void ResolveReferences()
    {
        pattern = _translationTokensRegex.Replace(pattern, match => match.Value.Trim('{', '}').Translate());
    }
}

public interface IDynamicNumberFormat
{
    public event Action OnChange;
}
