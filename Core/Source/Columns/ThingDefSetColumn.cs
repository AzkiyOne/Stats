using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.Widgets;
using Stats.Widgets.Filters;
using UnityEngine;

namespace Stats.Columns;

public abstract class ThingDefSetColumn<TRecord> : Column<TRecord, IReadOnlyCollection<Verse.ThingDef>?>
{
    private static readonly HashSet<Verse.ThingDef> _emptyThingDefHashSet = [];

    private readonly List<IReadOnlyCollection<Verse.ThingDef>> _cellValue;
    private readonly List<int> _cellThingDefsCount;
    private readonly List<Widget[]?> _cellIcons;
    private readonly List<float> _cellWidth;

    protected ThingDefSetColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefFilterOptions) : base(def, records)
    {
        string label = def.LabelCap;
        int capacity = records.Capacity;
        _cellValue = new List<IReadOnlyCollection<Verse.ThingDef>>(capacity);
        _cellThingDefsCount = new List<int>(capacity);
        _cellIcons = new List<Widget[]?>(capacity);
        _cellWidth = new List<float>(capacity);
        SortOptions = [
            new ColumnSortOption<int>(label, i => _cellThingDefsCount[i]),
        ];
        IEnumerable<NTMFilterOption<Verse.ThingDef>> thingDefNTMFilterOptions = thingDefFilterOptions
            .OrderBy(thingDef => thingDef.label)
            .Select<Verse.ThingDef, NTMFilterOption<Verse.ThingDef>>(
                thingDef => new(thingDef, thingDef.LabelCap, new Widgets_Legacy.ThingDefIcon(thingDef))
            );
        FilterOptions = [
            new MTMColumnFilterOption<Verse.ThingDef>(label, i => _cellValue[i], thingDefNTMFilterOptions),
        ];
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Left;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    public override void DrawCell(Rect rect, int index)
    {
        Widget[]? icons = _cellIcons[index];

        if (icons != null)
        {
            for (int i = 0; i < icons.Length; i++)
            {
                Widget icon = icons[i];
                // TODO: This can be optimized.
                rect = rect.CutLeft(out Rect iconRect, icon.Size.x);

                icon.Draw(iconRect);

                rect.xMin += GUIStyles.TableCell.ContentSpacing;
            }
        }
    }

    protected override float GetCellWidth(int i)
    {
        return _cellWidth[i];
    }

    private void GetCellValues(IReadOnlyCollection<Verse.ThingDef>? value)
    {

    }

    protected override void AddValue(IReadOnlyCollection<Verse.ThingDef>? value)
    {
        _cellValue.Add(value ?? _emptyThingDefHashSet);
    }

    protected override void RemoveValue(int i)
    {
        _cellValue.ReplaceWithLast(i);
        _cellThingDefsCount.ReplaceWithLast(i);
        _cellIcons.ReplaceWithLast(i);
        _cellWidth.ReplaceWithLast(i);
    }

    protected override void SetValue(int i, IReadOnlyCollection<Verse.ThingDef>? value)
    {
        throw new System.NotImplementedException();
    }
}
