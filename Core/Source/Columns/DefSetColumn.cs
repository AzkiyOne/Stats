using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.Widgets.Filters;
using UnityEngine;

namespace Stats.Columns;

public abstract class DefSetColumn<TRecord> : Column<TRecord>
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

    protected abstract IReadOnlyCollection<Verse.Def>? GetDefs(TRecord record);

    private void GetCellValues(TRecord record, out IReadOnlyCollection<Verse.Def>? defs, out string text, out float width)
    {
        defs = GetDefs(record);

        if (defs != null)
        {
            // TODO: This may be too slow.
            text = string.Join(" | ", defs.Select(def => def.LabelCap).OrderBy(text => text));
            width = text.CalcSize(GUIStyles.TableCell.String).x;
        }
        else
        {
            text = "";
            width = 0f;
        }
    }

    public override void Add(TRecord record)
    {
        GetCellValues(record, out IReadOnlyCollection<Verse.Def>? defs, out string text, out float width);

        _cellValue.Add(defs ?? _emptyDefHashSet);
        _cellText.Add(text);
        _cellWidth.Add(width);
    }

    public override void Refresh(int i, TRecord record)
    {
        GetCellValues(record, out IReadOnlyCollection<Verse.Def>? defs, out string text, out float width);

        _cellValue[i] = defs ?? _emptyDefHashSet;
        _cellText[i] = text;
        _cellWidth[i] = width;
    }

    public override void Remove(int i)
    {
        _cellValue.ReplaceWithLast(i);
        _cellText.ReplaceWithLast(i);
        _cellWidth.ReplaceWithLast(i);
    }
}
