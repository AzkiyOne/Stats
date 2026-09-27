using System.Collections.Generic;
using UnityEngine;

namespace Stats.Columns;

public readonly struct ThingDefSetColumnCell : IColumnCell
{
    public ThingDefSetColumnCell(IReadOnlyCollection<Verse.ThingDef?> value)
    {
        Value = value;
    }

    public float MinWidth { get; }

    public IReadOnlyCollection<Verse.ThingDef?>? Value { get; }

    public void Draw(Rect rect)
    {
        if (Value?.Count > 0)
        {
            rect.DrawLabel("TODO", GUIStyles.TableCell.String);
        }
    }
}
