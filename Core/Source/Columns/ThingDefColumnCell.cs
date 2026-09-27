using Stats.Extensions;
using Stats.Widgets;
using UnityEngine;
using static Stats.GUIStyles.TableCell;

namespace Stats.Columns;

public readonly struct ThingDefColumnCell : IColumnCell
{
    private readonly Widget? _icon;
    private readonly float _iconWidth;

    public ThingDefColumnCell(Verse.ThingDef value) : this(value, value.LabelCap, new ThingDefIconInteractive(value))
    {
    }

    public ThingDefColumnCell(Verse.ThingDef value, string text, Widget icon)
    {
        Value = value;
        float textWidth = Verse.Text.CalcSize(text).x;
        _icon = icon;
        _iconWidth = icon.Size.x;
        MinWidth = _iconWidth + ContentSpacing + textWidth;
    }

    public float MinWidth { get; }

    public Verse.ThingDef? Value { get; }

    public string? Text { get; }

    public void Draw(Rect rect)
    {
        if (Value != null)
        {
            rect.ContractedByObjectTableCellPadding()
                .CutLeft(out Rect iconRect, _iconWidth)
                .CutLeft(ContentSpacing)
                .TakeRest(out Rect labelRect);

            _icon!.Draw(iconRect);

            if (Event.current.type == EventType.Repaint)
            {
                Text!.Draw(labelRect, StringNoPad);
            }
        }
    }
}
