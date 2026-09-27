using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace Stats.Extensions;

public static class Verse_VerbProperties_List
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VerbProperties? Primary(this List<VerbProperties> verbs)
    {
        return verbs.FirstOrFallback(static verb => verb?.isPrimary == true);
    }
}
