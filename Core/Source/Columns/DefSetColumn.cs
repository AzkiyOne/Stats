using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.Widgets.Filters;
using UnityEngine;

namespace Stats.Columns;

public abstract class DefSetColumn<TRecord> : Column<TRecord, IReadOnlyCollection<Verse.Def>?>
{
    private static readonly HashSet<Verse.Def> _emptyDefHashSet = [];

    private readonly List<IReadOnlyCollection<Verse.Def>> _cellValue;
    private readonly List<string> _cellText;
    private readonly List<float> _cellWidth;

    protected DefSetColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.Def> defFilterOptions) : base(def, records)
    {
        string label = def.LabelCap;
        int capacity = records.Capacity;
        _cellValue = new List<IReadOnlyCollection<Verse.Def>>(capacity);
        _cellText = new List<string>(capacity);
        _cellWidth = new List<float>(capacity);
        SortOptions = [
            new ColumnSortOption<string>(label, i => _cellText[i])
        ];
        IEnumerable<NTMFilterOption<Verse.Def>> defNTMFilterOptions = defFilterOptions
            .OrderBy(def => def.label)
            .Select<Verse.Def, NTMFilterOption<Verse.Def>>(def => new(def, def.LabelCap));
        FilterOptions = [
            new MTMColumnFilterOption<Verse.Def>(label, i => _cellValue[i], defNTMFilterOptions)
        ];
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Left;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    public override void DrawCell(Rect rect, int i)
    {
        rect.DrawLabel(_cellText[i], GUIStyles.TableCell.String);
    }

    protected override float GetCellWidth(int i)
    {
        return _cellWidth[i];
    }

    private void GetCellValues(IReadOnlyCollection<Verse.Def>? value, out string text, out float width)
    {
        if (value != null)
        {
            // TODO: This may be too slow.
            text = string.Join(" | ", value.Select(def => def.LabelCap).OrderBy(text => text));
            width = text.CalcSize(GUIStyles.TableCell.String).x;
        }
        else
        {
            text = "";
            width = 0f;
        }
    }

    protected override void AddValue(IReadOnlyCollection<Verse.Def>? value)
    {
        GetCellValues(value, out string text, out float width);

        _cellValue.Add(value ?? _emptyDefHashSet);
        _cellText.Add(text);
        _cellWidth.Add(width);
    }

    protected override void RemoveValue(int i)
    {
        _cellValue.ReplaceWithLast(i);
        _cellText.ReplaceWithLast(i);
        _cellWidth.ReplaceWithLast(i);
    }

    protected override void SetValue(int i, IReadOnlyCollection<Verse.Def>? value)
    {
        GetCellValues(value, out string text, out float width);

        _cellValue[i] = value ?? _emptyDefHashSet;
        _cellText[i] = text;
        _cellWidth[i] = width;
    }
}
