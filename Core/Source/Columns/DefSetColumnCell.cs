using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using UnityEngine;

namespace Stats.Columns;

// TODO: Since all rows are now of constant height, we need to refactor this cell.
public readonly struct DefSetColumnCell : IColumnCell
{
    private static readonly GUIStyle _tagStyle = new(GUIStyles.TableCell.StringNoPad)
    {
        padding = new(GUIStyles.TableCell.String.padding.top, GUIStyles.TableCell.String.padding.top, 0, 0)
    };

    public DefSetColumnCell(IReadOnlyCollection<Verse.Def> value)
    {
        Value = value;
        if (value.Count > 0)
        {
            Text = string.Join(", ", value.Distinct().Select(def => def.LabelCap.ToString()).OrderBy(text => text));
            MinWidth = Text.CalcSize(GUIStyles.TableCell.StringNoPad).x;
        }
    }

    public float MinWidth { get; }

    public IReadOnlyCollection<Verse.Def>? Value { get; }

    public string? Text { get; }

    public void Draw(Rect rect)
    {
        if (Value?.Count > 0)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            // TODO: It's a draft. Not ready for production.
            foreach (var def in Value.Distinct().OrderBy(def => def.label))
            {
                if (def != null)
                {
                    string label = def.LabelCap;
                    rect = rect.CutLeft(out Rect labelRect, label.CalcSize(_tagStyle).x);
                    labelRect.Highlight();
                    labelRect.DrawLabel(label, _tagStyle);
                    rect = rect.CutLeft(GUIStyles.TableCell.ContentSpacing);
                }
            }
        }
    }
}
