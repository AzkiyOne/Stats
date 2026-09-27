using RimWorld;
using Stats.Columns;
using UnityEngine;
using Verse;

namespace Stats.Columns.ThingDef;

public readonly struct TechLevelColumnCell : IColumnCell
{
    public float MinWidth { get; }
    public bool IsRefreshable => false;
    public readonly TechLevel Value;

    private readonly string? _text;

    public TechLevelColumnCell(TechLevel techLevel)
    {
        Value = techLevel;
        if (techLevel != TechLevel.Undefined)
        {
            _text = techLevel.ToStringHuman().CapitalizeFirst();
            MinWidth = Text.CalcSize(_text).x;
        }
    }

    public void Draw(Rect rect)
    {
        if (Value != TechLevel.Undefined)
        {
            rect.DrawLabel(_text, GUIStyles.TableCell.String);
        }
    }
}
