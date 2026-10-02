using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using UnityEngine;
using Verse;

namespace Stats.Columns.BuildableDef;

public class StatColumn<TRecord> : Column<TRecord> where TRecord : IBuildableDefTableRecord
{
    private readonly StatDef _statDef;
    private readonly int _digits;
    private readonly string _formatString;
    private readonly List<decimal> _cellValue;
    private readonly List<string> _cellText;
    private readonly List<float> _cellWidth;
    private readonly List<Lazy<TipSignal>?> _cellTooltip;

    public StatColumn(StatColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
        string label = def.LabelCap;
        int capacity = records.Capacity;
        int digits = def.digits;
        string uom = def.uom;
        _statDef = def.stat;
        _digits = digits;
        if (digits == 0)
        {
            _formatString = $"0{uom}";
        }
        else
        {
            _formatString = $"0.{string.Join("", Enumerable.Repeat("0", digits))}{uom}";
        }
        _cellValue = new List<decimal>(capacity);
        _cellText = new List<string>(capacity);
        _cellWidth = new List<float>(capacity);
        _cellTooltip = new List<Lazy<TipSignal>?>(capacity);
        SortOptions = [
            new ColumnSortOption<decimal>(label, i => _cellValue[i])
        ];
        FilterOptions = [
            new NumberColumnFilterOption(label, i => _cellValue[i])
        ];
    }

    public override bool IsRefreshable => typeof(TRecord) is IThingTableRecord;

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Right;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    public override void DrawCell(Rect rect, int i)
    {
        _cellText[i].Draw(rect, GUIStyles.TableCell.Number);
    }

    protected override float GetCellWidth(int i)
    {
        return _cellWidth[i];
    }

    protected virtual StatRequest GetStatRequest(TRecord record)
    {
        return record.StatRequest;
    }

    private void GetCellValues(TRecord record, out decimal value, out string text, out Lazy<TipSignal>? tooltip, out float width)
    {
        StatRequest statRequest = GetStatRequest(record);

        if (_statDef.Worker.ShouldShowFor(statRequest))
        {
            float statValue = _statDef.Worker.GetValue(statRequest);

            value = statValue.ToDecimal(_digits);
            text = value.ToString(_formatString);
            width = text.CalcSize(GUIStyles.TableCell.Number).x;
            tooltip = new Lazy<TipSignal>(() => _statDef.Worker.GetExplanationFull(statRequest, ToStringNumberSense.Absolute, statValue));
        }
        else
        {
            value = 0m;
            text = "";
            width = 0f;
            tooltip = null;
        }
    }

    public override void Add(TRecord record)
    {
        GetCellValues(record, out decimal value, out string text, out Lazy<TipSignal>? tooltip, out float width);

        _cellValue.Add(value);
        _cellText.Add(text);
        _cellWidth.Add(width);
        _cellTooltip.Add(tooltip);
    }

    public override void Refresh(int i, TRecord record)
    {
        GetCellValues(record, out decimal value, out string text, out Lazy<TipSignal>? tooltip, out float width);

        _cellValue[i] = value;
        _cellText[i] = text;
        _cellWidth[i] = width;
        _cellTooltip[i] = tooltip;
    }

    public override void Remove(int i)
    {
        _cellValue.ReplaceWithLast(i);
        _cellText.ReplaceWithLast(i);
        _cellWidth.ReplaceWithLast(i);
        _cellTooltip.ReplaceWithLast(i);
    }
}
