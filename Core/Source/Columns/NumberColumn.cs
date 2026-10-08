using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Stats.Extensions;
using UnityEngine;

namespace Stats.Columns;

public abstract class NumberColumn<TRecord> : Column<TRecord>
{
    private readonly List<decimal> _cellValue;
    private List<string>? _cellText;
    private List<float>? _cellWidth;
    private readonly string _formatString;

    protected NumberColumn(ColumnDef def, List<TRecord> records, string formatString = "") : base(def, records)
    {
        string label = def.LabelCap;
        _formatString = formatString;
        _cellValue = new List<decimal>(records.Capacity);
        SortOptions = [
            new ColumnSortOption<decimal>(label, i => _cellValue[i])
        ];
        FilterOptions = [
            new NumberColumnFilterOption(label, i => _cellValue[i])
        ];
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Right;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    private List<string> CellText => _cellText ??= InitCellText();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private List<string> InitCellText()
    {
        return [.. _cellValue.Select(FormatValue)];
    }

    private List<float> CellWidth => _cellWidth ??= InitCellWidth();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private List<float> InitCellWidth()
    {
        return [.. CellText.Select(GetCellWidth)];
    }

    public override void DrawCell(Rect rect, int i)
    {
        rect.DrawLabel(CellText[i], GUIStyles.TableCell.Number);
    }

    protected override float GetCellWidth(int i)
    {
        return CellWidth[i];
    }

    private string FormatValue(decimal value)
    {
        return value == 0m ? "" : value.ToString(_formatString);
    }

    private float GetCellWidth(string text)
    {
        return text.CalcSize(GUIStyles.TableCell.Number).x;
    }

    public override void Hide()
    {
        _cellText = null;
        _cellWidth = null;
    }

    protected abstract decimal GetValueFromRecord(TRecord record);

    public override void Add(TRecord record)
    {
        decimal value = GetValueFromRecord(record);

        _cellValue.Add(value);
        _cellText?.Add(FormatValue(value));
        _cellWidth?.Add(GetCellWidth(CellText[^1]));
    }

    public override void Refresh(int i, TRecord record)
    {
        decimal value = GetValueFromRecord(record);

        _cellValue[i] = value;
        _cellText?[i] = FormatValue(value);
        _cellWidth?[i] = GetCellWidth(CellText[i]);
    }

    public override void Swap(int i1, int i2)
    {
        _cellValue.Swap(i1, i2);
        _cellText?.Swap(i1, i2);
        _cellWidth?.Swap(i1, i2);
    }

    public override void Replace(int i1, int i2)
    {
        _cellValue.Replace(i1, i2);
        _cellText?.Replace(i1, i2);
        _cellWidth?.Replace(i1, i2);
    }
}
