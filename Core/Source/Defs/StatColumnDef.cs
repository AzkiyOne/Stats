using RimWorld;
using Verse;

namespace Stats;

public class StatColumnDef : ColumnDef
{
#pragma warning disable CS8618
    public StatDef stat;
#pragma warning restore CS8618
    [DefaultValue(0)]
    public int digits;
    [MustTranslate]
    [DefaultValue("")]
    public string uom = "";

    public override void ResolveReferences()
    {
        if (string.IsNullOrEmpty(label))
        {
            label = stat.label;
        }

        if (string.IsNullOrEmpty(description))
        {
            description = stat.description;
        }

        base.ResolveReferences();
    }
}
