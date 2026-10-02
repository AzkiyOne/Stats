using System.Collections.Generic;
using Stats.Extensions;
using UnityEngine;
using Verse;

namespace Stats.Columns;

public abstract class BooleanColumn<TRecord> : Column<TRecord>
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

    protected abstract bool GetValueFromRecord(TRecord record);

    public override void Add(TRecord record)
    {
        bool value = GetValueFromRecord(record);

        _cellValue.Add(value);
    }

    public override void Refresh(int i, TRecord record)
    {
        bool value = GetValueFromRecord(record);

        _cellValue[i] = value;
    }

    public override void Remove(int index)
    {
        _cellValue.ReplaceWithLast(index);
    }

    public override void DrawCell(Rect rect, int i)
    {
        if (Event.current.type == EventType.Repaint && _cellValue[i])
        {
            rect.ContractedBy(GUIStyles.TableCell.PadLR, GUIStyles.TableCell.PadTB)
                .DrawTextureFitted(_textureTrue);
        }
    }

    public override float GetMaxCellWidth(List<int> recordIds)
    {
        return GetCellWidth(0);
    }

    protected override float GetCellWidth(int i)
    {
        return Verse.Text.LineHeight + GUIStyles.TableCell.PadHor;
    }
}
