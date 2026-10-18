using Verse;

namespace Stats.Extensions;

internal static class Verse_FloatRange
{
    internal static FloatRange MinHalf(this FloatRange range)
    {
        return new FloatRange(range.min, range.Average);
    }

    internal static FloatRange MaxHalf(this FloatRange range)
    {
        return new FloatRange(range.Average, range.max);
    }
}
