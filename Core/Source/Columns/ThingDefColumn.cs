using System.Collections.Generic;
using System.Linq;
using Stats.Widgets.Filters;

namespace Stats.Columns;

public abstract class ThingDefColumn<TRecord> : Column<TRecord, ThingDefColumnCell>
{
    private readonly string _label;

    protected ThingDefColumn(ColumnDef def) : base(def)
    {
        _label = def.LabelCap;
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Left;

    public override ICollection<ColumnSortOption> SortOptions => [
        new ColumnSortOption(_label, (i1, i2) => Comparer<string?>.Default.Compare(this[i1].Text, this[i2].Text))
    ];

    public override ICollection<ColumnFilterOption> FilterOptions => [
        new ColumnFilterOption(_label, () => new OTMFilter<Verse.ThingDef?>(i => this[i].Value, ThingDefFilterOptions))
    ];

    protected virtual IEnumerable<NTMFilterOption<Verse.ThingDef?>> ThingDefFilterOptions => ThingDefOptions
        .OrderBy(def => def?.label)
        .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
            thingDef => thingDef == null ? new() : new(thingDef, thingDef.LabelCap, new Widgets_Legacy.ThingDefIcon(thingDef))
        );

    protected abstract IEnumerable<Verse.ThingDef?> ThingDefOptions { get; }
}
