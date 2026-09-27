using System;
using System.Collections.Generic;

namespace Stats.Columns.ThingDef;

public class LabelColumnDef : ColumnDef
{
#pragma warning disable CS8618
    public Func<IEnumerable<Verse.ThingDef>> thingDefOptionsSource;
#pragma warning restore CS8618

    public IEnumerable<Verse.ThingDef> ThingDefOptions => thingDefOptionsSource();
}
