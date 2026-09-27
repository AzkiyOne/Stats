using System;
using System.Collections.Generic;
using Verse;

namespace Stats.Columns.ThingDef;

public class SizeColumnDef : ColumnDef
{
#pragma warning disable CS8618
    public Func<IEnumerable<IntVec2>> sizeOptionsSource;
#pragma warning restore CS8618

    public IEnumerable<IntVec2> SizeOptions => sizeOptionsSource();
}
