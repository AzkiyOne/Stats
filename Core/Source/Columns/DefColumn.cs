using System.Collections.Generic;
using System.Linq;
using Stats.Widgets.Filters;

namespace Stats.Columns;

public abstract class DefColumn<TRecord> : Column<TRecord, DefColumnCell>
{
    private readonly string _label;

    protected DefColumn(ColumnDef def) : base(def)
    {
        _label = def.LabelCap;
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Left;

    public override ICollection<ColumnSortOption> SortOptions => [
        new ColumnSortOption(_label, (i1, i2) => Comparer<string?>.Default.Compare(this[i1].Text, this[i2].Text))
    ];

    public override ICollection<ColumnFilterOption> FilterOptions => [
        new ColumnFilterOption(_label, () => new OTMFilter<Verse.Def?>(i => this[i].Value, DefFilterOptions))
    ];

    protected virtual IEnumerable<NTMFilterOption<Verse.Def?>> DefFilterOptions => DefOptions
        .OrderBy(def => def?.label)
        .Select<Verse.Def?, NTMFilterOption<Verse.Def?>>(def => def == null ? new() : new(def, def.LabelCap));

    protected abstract IEnumerable<Verse.Def?> DefOptions { get; }
}
