using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.Widgets;
using Stats.Widgets.Filters;
using UnityEngine;
using Verse;

namespace Stats.Columns;

public abstract class ThingDefColumn<TRecord> : Column<TRecord>
{
    private readonly List<Verse.ThingDef?> _cellValue;
    private readonly List<string> _cellText;
    private readonly List<Widget?> _cellIcon;
    private readonly List<float> _cellWidth;

    protected ThingDefColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef?> thingDefFilterOptions) : base(def, records)
    {
        string label = def.LabelCap;
        int capacity = records.Capacity;
        _cellValue = new List<Verse.ThingDef?>(capacity);
        _cellText = new List<string>(capacity);
        _cellIcon = new List<Widget?>(capacity);
        _cellWidth = new List<float>(capacity);
        SortOptions = [
            new ColumnSortOption<string>(label, i => _cellText[i])
        ];
        IEnumerable<NTMFilterOption<Verse.ThingDef?>> thingDefNTMFilterOptions = thingDefFilterOptions
            .OrderBy(thingDef => thingDef?.label)
            .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
                thingDef => thingDef == null ? new() : new(thingDef, thingDef.LabelCap, new Widgets_Legacy.ThingDefIcon(thingDef))
            );
        FilterOptions = [
            new OTMColumnFilterOption<Verse.ThingDef?>(label, i => _cellValue[i], thingDefNTMFilterOptions)
        ];
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Left;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    public override void DrawCell(Rect rect, int i)
    {
        string text = _cellText[i];
        Widget? icon = _cellIcon[i];

        if (icon != null)
        {
            rect.ContractedBy(GUIStyles.TableCell.PadLR, GUIStyles.TableCell.PadTB)
                .CutLeft(out Rect iconRect, icon.Size.x)
                .CutLeft(GUIStyles.TableCell.ContentSpacing)
                .TakeRest(out Rect labelRect);

            icon.Draw(iconRect);

            if (Event.current.type == EventType.Repaint)
            {
                text.Draw(labelRect, GUIStyles.TableCell.StringNoPad);
            }
        }
    }

    protected override float GetCellWidth(int i)
    {
        return _cellWidth[i];
    }

    protected abstract Verse.ThingDef? GetThingDef(TRecord record);

    private void GetCellValues(TRecord record, out Verse.ThingDef? thingDef, out string text, out Widget? icon, out float width)
    {
        thingDef = GetThingDef(record);

        if (thingDef != null)
        {
            text = thingDef.LabelCap;
            icon = new ThingDefIconInteractive(thingDef);
            float textWidth = text.CalcSize(GUIStyles.TableCell.StringNoPad).x;
            width = icon.Size.x + GUIStyles.TableCell.ContentSpacing + textWidth + GUIStyles.TableCell.PadHor;
        }
        else
        {
            text = "";
            icon = null;
            width = 0f;
        }
    }

    public override void Add(TRecord record)
    {
        GetCellValues(record, out Verse.ThingDef? thingDef, out string text, out Widget? icon, out float width);

        _cellValue.Add(thingDef);
        _cellText.Add(text);
        _cellIcon.Add(icon);
        _cellWidth.Add(width);
    }

    public override void Refresh(int i, TRecord record)
    {
        GetCellValues(record, out Verse.ThingDef? thingDef, out string text, out Widget? icon, out float width);

        _cellValue[i] = thingDef;
        _cellText[i] = text;
        _cellIcon[i] = icon;
        _cellWidth[i] = width;
    }

    public override void Remove(int i)
    {
        _cellValue.ReplaceWithLast(i);
        _cellText.ReplaceWithLast(i);
        _cellIcon.ReplaceWithLast(i);
        _cellWidth.ReplaceWithLast(i);
    }
}
