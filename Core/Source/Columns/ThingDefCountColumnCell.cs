using Stats.Extensions;
using Stats.Widgets;
using UnityEngine;
using Verse;
using static Stats.GUIStyles.TableCell;

namespace Stats.Columns;

public readonly struct ThingDefCountColumnCell : IColumnCell
{
    private readonly string? _text;
    private readonly ThingDefIcon? _icon;
    private readonly float _iconWidth;
    private readonly TipSignal _tooltip;

    public ThingDefCountColumnCell(Verse.ThingDef thingDef, float count) : this(thingDef, count.ToDecimal(0)) { }

    public ThingDefCountColumnCell(Verse.ThingDef thingDef, decimal count)
    {
        ThingDef = thingDef;
        ThingDefLabel = thingDef.label;
        _tooltip = thingDef.LabelCap;
        Count = count;
        _text = count.ToString();
        float textWidth = _text.CalcSize(NumberNoPad).x;
        _icon = new ThingDefIcon(thingDef);
        _iconWidth = _icon.Size.x;
        MinWidth = textWidth + ContentSpacing + _iconWidth;
    }

    public float MinWidth { get; }

    public Verse.ThingDef? ThingDef { get; }

    public string? ThingDefLabel { get; }

    public decimal Count { get; }

    public void Draw(Rect rect)
    {
        if (ThingDef != null && Count != 0m)
        {
            rect.ContractedByObjectTableCellPadding()
                .CutRight(out Rect iconRect, _iconWidth)
                .CutRight(ContentSpacing)
                .TakeRest(out Rect labelRect);

            if (Event.current.type == EventType.Repaint)
            {
                _text!.Draw(labelRect, NumberNoPad);
                _icon!.Draw(iconRect);
                iconRect.Tip(_tooltip);
            }

            bool iconWasClicked = iconRect.DrawButtonGhostly();

            if (iconWasClicked)
            {
                ThingDef.OpenInfoDialog();
            }
        }
    }
}
