using UnityEngine;

namespace Stats.Columns;

public readonly struct DefColumnCell : IColumnCell
{
    public DefColumnCell(Verse.Def value)
    {
        Value = value;
        Text = value.LabelCap;
        MinWidth = Verse.Text.CalcSize(Text).x;
    }

    public float MinWidth { get; }

    public Verse.Def? Value { get; }

    public string? Text { get; }

    public void Draw(Rect rect)
    {
        if (Text != null)
        {
            rect.DrawLabel(Text, GUIStyles.TableCell.String);
        }
    }
}
