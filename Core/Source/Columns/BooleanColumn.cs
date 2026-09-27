using System.Collections.Generic;
using Stats.Extensions;
using UnityEngine;

namespace Stats.Columns;

public abstract class BooleanColumn<TRecord> : Column<TRecord, bool>
{
    private static readonly Texture2D _textureTrue = Verse.Widgets.CheckboxOnTex;
    private readonly List<bool> _cellValueComp;

    protected BooleanColumn(ColumnDef def, List<TRecord> records) : base(def, records)
    {
        string name = def.LabelCap;
        _cellValueComp = new List<bool>(records.Capacity);
        SortOptions = [
            new ColumnSortOption<bool>(name, _cellValueComp)
        ];
        FilterOptions = [
            new BooleanColumnFilterOption(name, _cellValueComp)
        ];
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Middle;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    protected override void AddValue(bool value)
    {
        _cellValueComp.Add(value);
    }

    protected override void SetValue(int i, bool value)
    {
        _cellValueComp[i] = value;
    }

    protected override void RemoveValue(int index)
    {
        _cellValueComp.ReplaceWithLast(index);
    }

    public override void DrawCell(Rect rect, int i)
    {
        if (Event.current.type == EventType.Repaint && _cellValueComp[i])
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

    public override void Hide()
    {
    }
}
