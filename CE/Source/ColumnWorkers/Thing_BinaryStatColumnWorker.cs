using System.Linq;
using System.Text.RegularExpressions;
using RimWorld;
using Verse;

namespace Stats.Compat.CE;

public abstract class Thing_BinaryStatColumnWorker : StatColumn
{
    protected static readonly Regex NonZeroNumberRegex = new(@"[1-9]{1}", RegexOptions.Compiled);
    protected readonly char Separator;
    protected Thing_BinaryStatColumnWorker(BinaryStatColumnDef columnDef) : base(columnDef)
    {
        Separator = columnDef.statValueSeparator[0];
    }
    protected override string? GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest statRequest)
    {
        var label = base.GetStatDrawEntryLabel(stat, value, numberSense, statRequest);

        // We do not check whether statValue is 0 here,
        // because since this type of stat is often times used
        // only for display purposes it can have a value of 0,
        // but display non-zero value.

        if (label != null)
        {
            return ParsePartFromLabel(label);
        }

        return null;
    }
    protected abstract string? ParsePartFromLabel(string label);
}

public sealed class Thing_BinaryStatColumnWorker_Left : Thing_BinaryStatColumnWorker
{
    public Thing_BinaryStatColumnWorker_Left(BinaryStatColumnDef columnDef) : base(columnDef)
    {
    }
    protected override string? ParsePartFromLabel(string label)
    {
        label = label.Split(Separator).First();

        if (NonZeroNumberRegex.IsMatch(label))
        {
            return label.TrimEnd();
        }

        return null;
    }
}

public sealed class Thing_BinaryStatColumnWorker_Right : Thing_BinaryStatColumnWorker
{
    public Thing_BinaryStatColumnWorker_Right(BinaryStatColumnDef columnDef) : base(columnDef)
    {
    }
    protected override string? ParsePartFromLabel(string label)
    {
        label = label.Split(Separator).Last();

        if (NonZeroNumberRegex.IsMatch(label))
        {
            return label.TrimStart();
        }

        return null;
    }
}
