using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns;
using Stats.TableRecords;
using Stats.Widgets.Filters;
using Verse;

namespace Stats.Columns.ThingDef;

public abstract class TechLevelColumn<TRecord> : Column<TRecord, TechLevelColumnCell> where TRecord : IThingDefTableRecord
{
    private readonly string _label;
    private readonly IEnumerable<NTMFilterOption<TechLevel>> _filterOptions;

    public TechLevelColumn(ColumnDef def, ThingDefTableDef tableDef) : base(def)
    {
        _label = def.LabelCap;
        _filterOptions = tableDef.ThingDefOptions
            .Select(GetTechLevel)
            .Distinct()
            .OrderBy(techLevel => techLevel)
            .Select(techLevel => new NTMFilterOption<TechLevel>(techLevel, techLevel.ToStringHuman().CapitalizeFirst()));
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Left;

    public override ICollection<ColumnSortOption> SortOptions => [
        new ColumnSortOption(_label, (i1, i2) => this[i1].Value.CompareTo(this[i2].Value))
    ];

    public override ICollection<ColumnFilterOption> FilterOptions => [
        new ColumnFilterOption(_label, () => new OTMFilter<TechLevel>(i => this[i].Value, _filterOptions))
    ];

    protected override TechLevelColumnCell MakeCell(TRecord record)
    {
        TechLevel techLevel = GetTechLevel(record.ThingDef);

        return new TechLevelColumnCell(techLevel);
    }

    private static TechLevel GetTechLevel(Verse.ThingDef thingDef)
    {
        return thingDef.techLevel;
    }
}
