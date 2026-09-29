using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.Widgets.Filters;
using UnityEngine;

namespace Stats.Columns;

public abstract class DefColumn<TRecord> : Column<TRecord, Verse.Def?>
{
    private readonly List<Verse.Def?> _cellValue;
    private readonly List<string> _cellText;
    private readonly List<float> _cellWidth;

    protected DefColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.Def?> defFilterOptions) : base(def, records)
    {
        string label = def.LabelCap;
        int capacity = records.Capacity;
        _cellValue = new List<Verse.Def?>(capacity);
        _cellText = new List<string>(capacity);
        _cellWidth = new List<float>(capacity);
        SortOptions = [
            new ColumnSortOption<string>(label, i => _cellText[i])
        ];
        IEnumerable<NTMFilterOption<Verse.Def?>> defNTMFilterOptions = defFilterOptions
            .OrderBy(def => def?.label)
            .Select<Verse.Def?, NTMFilterOption<Verse.Def?>>(def => def == null ? new() : new(def, def.LabelCap));
        FilterOptions = [
            new OTMColumnFilterOption<Verse.Def?>(label, i => _cellValue[i], defNTMFilterOptions)
        ];
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Left;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    public override void DrawCell(Rect rect, int i)
    {
        string text = _cellText[i];

        rect.DrawLabel(text, GUIStyles.TableCell.String);
    }

    protected override float GetCellWidth(int i)
    {
        return _cellWidth[i];
    }

    private void GetCellValues(Verse.Def? def, out string text, out float width)
    {
        text = def?.LabelCap ?? "";
        width = text.CalcSize(GUIStyles.TableCell.String).x;
    }

    protected override void AddValue(Verse.Def? def)
    {
        GetCellValues(def, out string text, out float width);

        _cellValue.Add(def);
        _cellText.Add(text);
        _cellWidth.Add(width);
    }

    protected override void RemoveValue(int i)
    {
        _cellValue.ReplaceWithLast(i);
        _cellText.ReplaceWithLast(i);
        _cellWidth.ReplaceWithLast(i);
    }

    protected override void SetValue(int i, Verse.Def? def)
    {
        GetCellValues(def, out string text, out float width);

        _cellValue[i] = def;
        _cellText[i] = text;
        _cellWidth[i] = width;
    }
}
