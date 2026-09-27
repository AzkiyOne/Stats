using System.Collections.Generic;
using System.Linq;
using Stats.Columns;
using Stats.TableRecords;
using Stats.Widgets.Filters;
using Verse;

namespace Stats.Columns.Def;

public sealed class ModContentPackColumn<TRecord> : Column<TRecord, ModContentPackColumnCell> where TRecord : IDefTableRecord
{
    private readonly string _label;
    private readonly IEnumerable<NTMFilterOption<ModContentPack?>> _filterOptions;

    public ModContentPackColumn(ModContentPackColumnDef def) : base(def)
    {
        _label = def.LabelCap;
        _filterOptions = def.ModContentPackOptions
            .OrderBy(mod => mod?.Name)
            .Select<ModContentPack?, NTMFilterOption<ModContentPack?>>(
                mod => mod == null ? new() : new(mod, mod.Name, null, mod.PackageIdPlayerFacing)
            );
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Left;

    public override ICollection<ColumnSortOption> SortOptions => [
        new ColumnSortOption(_label, (i1, i2) => Comparer<string?>.Default.Compare(this[i1].Text, this[i2].Text))
    ];

    public override ICollection<ColumnFilterOption> FilterOptions => [
        new ColumnFilterOption(_label, () => new OTMFilter<ModContentPack?>(i => this[i].Value, _filterOptions))
    ];

    protected override ModContentPackColumnCell MakeCell(TRecord record)
    {
        Verse.Def def = record.Def;
        ModContentPack? modContentPack = def.modContentPack;

        if (modContentPack != null)
        {
            return new ModContentPackColumnCell(modContentPack);
        }

        return default;
    }
}
