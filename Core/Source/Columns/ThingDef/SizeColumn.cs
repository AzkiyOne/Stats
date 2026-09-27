using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Columns;
using Stats.TableRecords;
using Stats.Widgets.Filters;
using Verse;

namespace Stats.Columns.ThingDef;

public sealed class SizeColumn<TRecord> : Column<TRecord, SizeColumnCell> where TRecord : IThingDefTableRecord
{
    private readonly string _label;
    private readonly IEnumerable<NTMFilterOption<decimal>> _filterOptions;

    public SizeColumn(SizeColumnDef def) : base(def)
    {
        _label = def.LabelCap;
        _filterOptions = def.SizeOptions
            .Select(NormalizeSize)
            .Distinct()
            .OrderBy(size => size.Area)
            .Select(size => new NTMFilterOption<decimal>(size.Area, size.ToStringCross()));
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Right;

    public override ICollection<ColumnSortOption> SortOptions => [
        new ColumnSortOption(_label, (i1, i2) => this[i1].Value.CompareTo(this[i2].Value))
    ];

    public override ICollection<ColumnFilterOption> FilterOptions => [
        new ColumnFilterOption(_label, () => new OTMFilter<decimal>(i => this[i].Value, _filterOptions))
    ];

    protected override SizeColumnCell MakeCell(TRecord record)
    {
        IntVec2 size = NormalizeSize(record.ThingDef.size);

        return new SizeColumnCell(size);
    }

    private static IntVec2 NormalizeSize(IntVec2 vec2)
    {
        // Because 4x5 == 5x4.
        return new IntVec2(Math.Max(vec2.x, vec2.z), Math.Min(vec2.x, vec2.z));
    }
}
