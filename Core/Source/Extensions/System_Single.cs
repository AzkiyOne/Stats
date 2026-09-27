using System;
using System.Runtime.CompilerServices;

namespace Stats.Extensions;

public static class System_Single
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ToDecimal(this float value, int digits = 0)
    {
        // "When you convert float or double to decimal, the source value is converted
        // to decimal representation and rounded to the nearest number after
        // the 28th decimal place if necessary."
        //
        // Keyword is "if".
        return (decimal)Math.Round(value, digits);
    }
}
