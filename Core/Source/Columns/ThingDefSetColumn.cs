using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.Widgets;
using Stats.Widgets.Filters;
using UnityEngine;
using Verse;

namespace Stats.Columns;

public abstract class ThingDefSetColumn<TRecord> : Column<TRecord>
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
            rect = rect.ContractedBy(GUIStyles.TableCell.PadLR, GUIStyles.TableCell.PadTB);

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

    protected abstract IReadOnlyCollection<Verse.ThingDef>? GetThingDefs(TRecord record);

    private void GetCellValues(
        TRecord record,
        out IReadOnlyCollection<Verse.ThingDef>? value,
        out int thingDefsCount,
        out Widget[]? icons,
        out float width)
    {
        value = GetThingDefs(record);

        if (value != null)
        {
            thingDefsCount = value.Count;
            icons = new Widget[thingDefsCount];

            int i = 0;
            // TODO: Can we do sorting faster or get rid of it?
            foreach (Verse.ThingDef thingDef in value.OrderBy(thingDef => thingDef.label))
            {
                icons[i] = new ThingDefIconInteractive(thingDef);

                i++;
            }

            float iconWidth = GUIStyles.Text.LineHeight;
            width = iconWidth * thingDefsCount + GUIStyles.TableCell.ContentSpacing * (thingDefsCount - 1) + GUIStyles.TableCell.PadHor;
        }
        else
        {
            thingDefsCount = 0;
            icons = null;
            width = 0f;
        }
    }

    public override void Add(TRecord record)
    {
        GetCellValues(
            record,
            out IReadOnlyCollection<Verse.ThingDef>? value,
            out int thingDefsCount,
            out Widget[]? icons,
            out float width);

        _cellValue.Add(value ?? _emptyThingDefHashSet);
        _cellThingDefsCount.Add(thingDefsCount);
        _cellIcons.Add(icons);
        _cellWidth.Add(width);
    }

    public override void Refresh(int i, TRecord record)
    {
        GetCellValues(
            record,
            out IReadOnlyCollection<Verse.ThingDef>? value,
            out int thingDefsCount,
            out Widget[]? icons,
            out float width);

        _cellValue[i] = value ?? _emptyThingDefHashSet;
        _cellThingDefsCount[i] = thingDefsCount;
        _cellIcons[i] = icons;
        _cellWidth[i] = width;
    }

    public override void Swap(int i1, int i2)
    {
        _cellValue.Swap(i1, i2);
        _cellThingDefsCount.Swap(i1, i2);
        _cellIcons.Swap(i1, i2);
        _cellWidth.Swap(i1, i2);
    }

    public override void Remove(int i)
    {
        _cellValue.ReplaceWithLast(i);
        _cellThingDefsCount.ReplaceWithLast(i);
        _cellIcons.ReplaceWithLast(i);
        _cellWidth.ReplaceWithLast(i);
    }
}
