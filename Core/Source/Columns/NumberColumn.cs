using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Stats.Extensions;
using UnityEngine;

namespace Stats.Columns;

public abstract class NumberColumn<TRecord> : Column<TRecord, decimal>
{
    private readonly List<decimal> _cellValueComp;
    private List<string>? _cellTextComp;
    private List<float>? _cellWidthComp;
    private readonly string _formatString;

    protected NumberColumn(ColumnDef def, List<TRecord> records, string formatString = "") : base(def, records)
    {
        string name = def.LabelCap;
        _formatString = formatString;
        _cellValueComp = new List<decimal>(records.Capacity);
        SortOptions = [
            new ColumnSortOption<decimal>(name, _cellValueComp)
        ];
        FilterOptions = [
            new NumberColumnFilterOption(name, _cellValueComp)
        ];
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Right;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    private List<string> CellTextComp => _cellTextComp ??= InitCellTextComp();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private List<string> InitCellTextComp()
    {
        return [.. _cellValueComp.Select(FormatValue)];
    }

    private List<float> CellWidthComp => _cellWidthComp ??= InitCellWidthComp();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private List<float> InitCellWidthComp()
    {
        return [.. CellTextComp.Select(GetCellWidth)];
    }

    public override void DrawCell(Rect rect, int i)
    {
        rect.DrawLabel(CellTextComp[i], GUIStyles.TableCell.Number);
    }

    protected override float GetCellWidth(int i)
    {
        return CellWidthComp[i];
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
        _cellTextComp = null;
        _cellWidthComp = null;
    }

    protected override void AddValue(decimal value)
    {
        _cellValueComp.Add(value);
        _cellTextComp?.Add(FormatValue(value));
        _cellWidthComp?.Add(GetCellWidth(CellTextComp[^1]));
    }

    protected override void SetValue(int i, decimal value)
    {
        _cellValueComp[i] = value;
        _cellTextComp?[i] = FormatValue(value);
        _cellWidthComp?[i] = GetCellWidth(CellTextComp[i]);
    }

    protected override void RemoveValue(int index)
    {
        _cellValueComp.ReplaceWithLast(index);
        _cellTextComp?.ReplaceWithLast(index);
        _cellWidthComp?.ReplaceWithLast(index);
    }
}
