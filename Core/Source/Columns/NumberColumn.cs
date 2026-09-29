using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Stats.Extensions;
using UnityEngine;

namespace Stats.Columns;

public abstract class NumberColumn<TRecord> : Column<TRecord, decimal>
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

    protected override void AddValue(decimal value)
    {
        _cellValue.Add(value);
        _cellText?.Add(FormatValue(value));
        _cellWidth?.Add(GetCellWidth(CellText[^1]));
    }

    protected override void SetValue(int i, decimal value)
    {
        _cellValue[i] = value;
        _cellText?[i] = FormatValue(value);
        _cellWidth?[i] = GetCellWidth(CellText[i]);
    }

    protected override void RemoveValue(int index)
    {
        _cellValue.ReplaceWithLast(index);
        _cellText?.ReplaceWithLast(index);
        _cellWidth?.ReplaceWithLast(index);
    }
}
