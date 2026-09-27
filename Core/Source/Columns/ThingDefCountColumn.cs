using System.Collections.Generic;
using System.Linq;
using Stats.Widgets.Filters;

namespace Stats.Columns;

public abstract class ThingDefCountColumn<TRecord> : Column<TRecord, ThingDefCountColumnCell>
{
    protected ThingDefCountColumn(ColumnDef def) : base(def)
    {
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Right;

    public override ICollection<ColumnSortOption> SortOptions => [
        new ColumnSortOption("Amount", (i1, i2) => this[i1].Count.CompareTo(this[i2].Count)),
        new ColumnSortOption("Label", (i1, i2) => Comparer<string?>.Default.Compare(this[i1].ThingDefLabel, this[i2].ThingDefLabel))
    ];

    public override ICollection<ColumnFilterOption> FilterOptions => [
        new ColumnFilterOption("Amount", () => new NumberFilter(i => this[i].Count)),
        new ColumnFilterOption("Type", () => new OTMFilter<Verse.ThingDef?>(i => this[i].ThingDef, ThingDefFilterOptions))
    ];

    protected virtual IEnumerable<NTMFilterOption<Verse.ThingDef?>> ThingDefFilterOptions => ThingDefOptions
        .OrderBy(def => def?.label)
        .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
            thingDef => thingDef == null ? new() : new(thingDef, thingDef.LabelCap, new Widgets_Legacy.ThingDefIcon(thingDef))
        );

    protected abstract IEnumerable<Verse.ThingDef?> ThingDefOptions { get; }
}
