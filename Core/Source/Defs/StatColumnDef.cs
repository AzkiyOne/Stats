using RimWorld;
using Stats.NumberFormats;

namespace Stats;

public class StatColumnDef : ColumnDef
{
#pragma warning disable CS8618
    public StatDef stat;
    public NumberFormatProps? format;
#pragma warning restore CS8618

    public NumberFormat NumberFormat => format?.NumberFormatInstance ?? new Standard();

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

        format?.ResolveReferences();

        base.ResolveReferences();
    }
}
