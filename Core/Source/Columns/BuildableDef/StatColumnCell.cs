using System;
using System.Text.RegularExpressions;
using RimWorld;
using Stats.Columns;
using UnityEngine;
using Verse;

namespace Stats.Columns.BuildableDef;

// TODO: Use NumberCell as inner cell to avoid code duplication.
public readonly struct StatColumnCell : IColumnCell
{
    private const ToStringNumberSense _ToStringNumberSense = ToStringNumberSense.Absolute;

    private static readonly Regex _numberRegex = new(@"(-?[0-9]+\.?[0-9]*).*", RegexOptions.Compiled);

    private readonly string? _text;
    private readonly Lazy<TipSignal>? _tooltip;

    public StatColumnCell(float statValue, StatRequest statRequest, StatDef statDef)
    {
        if (statValue != 0f)
        {
            StatValue = statValue;
            _text = statDef.Worker.GetStatDrawEntryLabel(statDef, statValue, _ToStringNumberSense, statRequest);
            MinWidth = Text.CalcSize(_text).x;
            Match match = _numberRegex.Match(_text);
            if (match.Success)
            {
                Value = decimal.Parse(match.Groups[1].Captures[0].Value);
            }
            _tooltip = new Lazy<TipSignal>(() => statDef.Worker.GetExplanationFull(statRequest, _ToStringNumberSense, statValue));
        }
    }

    public float MinWidth { get; }

    public decimal Value { get; }

    public float StatValue { get; }

    public void Draw(Rect rect)
    {
        if (_text != null)
        {
            if (Mouse.IsOver(rect) && _tooltip != null)
            {
                rect.Tip(_tooltip.Value);
            }

            rect.DrawLabel(_text, GUIStyles.TableCell.Number);
        }
    }
}
