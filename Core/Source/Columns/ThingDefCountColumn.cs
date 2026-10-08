using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.Widgets;
using Stats.Widgets.Filters;
using UnityEngine;
using Verse;

namespace Stats.Columns;

public abstract class ThingDefCountColumn<TRecord> : Column<TRecord>
{
    private readonly List<decimal> _cellCount;
    private readonly List<Verse.ThingDef?> _cellThingDef;
    private readonly List<string> _cellThingDefLabel;
    private readonly List<float> _cellWidth;
    private readonly List<CellDrawData?> _cellDrawData;

    protected ThingDefCountColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef?> thingDefFilterOptions) : base(def, records)
    {
        string label = def.LabelCap;
        int capacity = records.Capacity;
        _cellCount = new List<decimal>(capacity);
        _cellThingDef = new List<Verse.ThingDef?>(capacity);
        _cellThingDefLabel = new List<string>(capacity);
        _cellWidth = new List<float>(capacity);
        _cellDrawData = new List<CellDrawData?>(capacity);
        SortOptions = [
            new ColumnSortOption<decimal>("Amount", i => _cellCount[i]),
            new ColumnSortOption<string>("ThingDef Label", i => _cellThingDefLabel[i])
        ];
        IEnumerable<NTMFilterOption<Verse.ThingDef?>> thingDefNTMFilterOptions = thingDefFilterOptions
            .OrderBy(def => def?.label)
            .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
                thingDef => thingDef == null ? new() : new(thingDef, thingDef.LabelCap, new Widgets_Legacy.ThingDefIcon(thingDef))
            );
        FilterOptions = [
            new NumberColumnFilterOption("Amount", i => _cellCount[i]),
            new OTMColumnFilterOption<Verse.ThingDef?>("Type", i => _cellThingDef[i], thingDefNTMFilterOptions),
            new StringColumnFilterOption("ThingDef Label", i => _cellThingDefLabel[i]),
        ];
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Right;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    public override void DrawCell(Rect rect, int i)
    {
        CellDrawData? drawData = _cellDrawData[i];

        if (drawData.HasValue)
        {
            (string text, Widget icon) = drawData.Value;

            rect.ContractedBy(GUIStyles.TableCell.PadLR, GUIStyles.TableCell.PadTB)
                .CutRight(out Rect iconRect, icon.Size.x)
                .CutRight(GUIStyles.TableCell.ContentSpacing)
                .TakeRest(out Rect labelRect);

            if (Event.current.type == EventType.Repaint)
            {
                text.Draw(labelRect, GUIStyles.TableCell.NumberNoPad);
            }

            icon.Draw(iconRect);
        }
    }

    protected override float GetCellWidth(int i)
    {
        return _cellWidth[i];
    }

    protected abstract ThingDefCount? GetThingDefCount(TRecord record);

    private void GetCellValues(
        TRecord record,
        out decimal count,
        out Verse.ThingDef? thingDef,
        out string thingDefLabel,
        out CellDrawData? cellDrawData,
        out float cellWidth)
    {
        ThingDefCount? thingDefCount = GetThingDefCount(record);

        if (thingDefCount.HasValue)
        {
            count = thingDefCount.Value.Count;
            thingDef = thingDefCount.Value.ThingDef;
            thingDefLabel = thingDef.LabelCap;
            string cellText = count.ToString();
            Widget cellIcon = new ThingDefIconInteractive(thingDef);
            cellDrawData = new CellDrawData(cellText, cellIcon);
            float cellTextWidth = cellText.CalcSize(GUIStyles.TableCell.NumberNoPad).x;
            float cellIconWidth = cellIcon.Size.x;
            cellWidth = cellTextWidth + GUIStyles.TableCell.ContentSpacing + cellIconWidth + GUIStyles.TableCell.PadHor;
        }
        else
        {
            count = 0m;
            thingDef = null;
            thingDefLabel = "";
            cellDrawData = null;
            cellWidth = 0f;
        }
    }

    public override void Add(TRecord record)
    {
        GetCellValues(
            record,
            out decimal count,
            out Verse.ThingDef? thingDef,
            out string thingDefLabel,
            out CellDrawData? cellDrawData,
            out float cellWidth);

        _cellCount.Add(count);
        _cellThingDef.Add(thingDef);
        _cellThingDefLabel.Add(thingDefLabel);
        _cellDrawData.Add(cellDrawData);
        _cellWidth.Add(cellWidth);
    }

    public override void Refresh(int i, TRecord record)
    {
        GetCellValues(
            record,
            out decimal count,
            out Verse.ThingDef? thingDef,
            out string thingDefLabel,
            out CellDrawData? cellDrawData,
            out float cellWidth);

        _cellCount[i] = count;
        _cellThingDef[i] = thingDef;
        _cellThingDefLabel[i] = thingDefLabel;
        _cellDrawData[i] = cellDrawData;
        _cellWidth[i] = cellWidth;
    }

    public override void Swap(int i1, int i2)
    {
        _cellCount.Swap(i1, i2);
        _cellThingDef.Swap(i1, i2);
        _cellThingDefLabel.Swap(i1, i2);
        _cellDrawData.Swap(i1, i2);
        _cellWidth.Swap(i1, i2);
    }

    public override void Replace(int i1, int i2)
    {
        _cellCount.Replace(i1, i2);
        _cellThingDef.Replace(i1, i2);
        _cellThingDefLabel.Replace(i1, i2);
        _cellDrawData.Replace(i1, i2);
        _cellWidth.Replace(i1, i2);
    }

    private readonly record struct CellDrawData(string Text, Widget Icon);
}
