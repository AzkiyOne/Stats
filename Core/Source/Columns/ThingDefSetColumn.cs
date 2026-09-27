using System.Collections.Generic;
using System.Linq;
using Stats.Widgets.Filters;

namespace Stats.Columns;

public abstract class ThingDefSetColumn<TRecord> : Column<TRecord, ThingDefSetColumnCell>
{
    private static readonly HashSet<Verse.ThingDef> _emptyThingDefHashSet = [];

    private readonly string _label;

    protected ThingDefSetColumn(ColumnDef def) : base(def)
    {
        _label = def.LabelCap;
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Left;

    public override ICollection<ColumnSortOption> SortOptions => [
        // TODO: Figure out how to efficiently compare cells so that cells with equal values will be grouped together.
        //new ColumnSortOption(_label, (i1, i2) => i1.CompareTo(i2))
    ];

    public override ICollection<ColumnFilterOption> FilterOptions => [
        new ColumnFilterOption(_label, () => new MTMFilter<Verse.ThingDef?>(row => this[row].Value ?? _emptyThingDefHashSet, ThingDefFilterOptions))
    ];

    protected virtual IEnumerable<NTMFilterOption<Verse.ThingDef?>> ThingDefFilterOptions => ThingDefOptions
        .OrderBy(def => def?.label)
        .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
            thingDef => thingDef == null ? new() : new(thingDef, thingDef.LabelCap, new Widgets_Legacy.ThingDefIcon(thingDef))
        );

    protected abstract IEnumerable<Verse.ThingDef?> ThingDefOptions { get; }
}
