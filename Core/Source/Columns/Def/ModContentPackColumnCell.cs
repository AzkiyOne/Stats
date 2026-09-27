using Stats.Columns;
using UnityEngine;
using Verse;

namespace Stats.Columns.Def;

public readonly struct ModContentPackColumnCell : IColumnCell
{
    private readonly TipSignal _tooltip;

    public ModContentPackColumnCell(ModContentPack mod)
    {
        Value = mod;
        Text = mod.Name;
        MinWidth = Verse.Text.CalcSize(Text).x;
        _tooltip = mod.PackageIdPlayerFacing;
    }

    public float MinWidth { get; }

    public ModContentPack? Value { get; }

    public string? Text { get; }

    public void Draw(Rect rect)
    {
        if (Text != null)
        {
            rect.DrawLabel(Text, GUIStyles.TableCell.String)
                .Tip(_tooltip);
        }
    }
}
