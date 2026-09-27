using Stats.Columns;
using UnityEngine;
using Verse;

namespace Stats.Columns.ThingDef;

public readonly struct SizeColumnCell : IColumnCell
{
    private readonly string? _text;

    public SizeColumnCell(IntVec2 size)
    {
        Value = size.Area;

        if (Value != 0m)
        {
            _text = size.ToStringCross();
            MinWidth = Text.CalcSize(_text).x;
        }
    }

    public float MinWidth { get; }

    public decimal Value { get; }

    public void Draw(Rect rect)
    {
        if (_text != null)
        {
            rect.DrawLabel(_text, GUIStyles.TableCell.Number);
        }
    }
}
