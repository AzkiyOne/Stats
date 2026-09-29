using System.Collections.Generic;
using Stats.Extensions;
using UnityEngine;

namespace Stats.Columns;

public abstract class BooleanColumn<TRecord> : Column<TRecord, bool>
{
    private static readonly Texture2D _textureTrue = Verse.Widgets.CheckboxOnTex;
    private readonly List<bool> _cellValue;

    protected BooleanColumn(ColumnDef def, List<TRecord> records) : base(def, records)
    {
        string label = def.LabelCap;
        _cellValue = new List<bool>(records.Capacity);
        SortOptions = [
            new ColumnSortOption<bool>(label, i => _cellValue[i])
        ];
        FilterOptions = [
            new BooleanColumnFilterOption(label, i => _cellValue[i])
        ];
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Middle;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    protected override void AddValue(bool value)
    {
        _cellValue.Add(value);
    }

    protected override void SetValue(int i, bool value)
    {
        _cellValue[i] = value;
    }

    protected override void RemoveValue(int index)
    {
        _cellValue.ReplaceWithLast(index);
    }

    public override void DrawCell(Rect rect, int i)
    {
        if (Event.current.type == EventType.Repaint && _cellValue[i])
        {
            rect.ContractedByObjectTableCellPadding()
                .DrawTextureFitted(_textureTrue);
        }
    }

    public override float GetMinWidth(List<int> recordIds)
    {
        return GetCellWidth(0);
    }

    protected override float GetCellWidth(int i)
    {
        return Verse.Text.LineHeight;
    }
}
