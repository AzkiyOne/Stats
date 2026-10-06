using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets.Filters;
using UnityEngine;
using Verse;

namespace Stats.Columns.Def;

public sealed class ModContentPackColumn<TRecord> : Column<TRecord> where TRecord : IDefTableRecord
{
    private readonly List<ModContentPack?> _cellValue;
    private readonly List<string> _cellText;
    private readonly List<float> _cellWidth;

    public ModContentPackColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.Def> defs) : base(def, records)
    {
        string label = def.LabelCap;
        int capacity = records.Capacity;
        _cellValue = new List<ModContentPack?>(capacity);
        _cellText = new List<string>(capacity);
        _cellWidth = new List<float>(capacity);
        SortOptions = [
            new ColumnSortOption<string>(label, i => _cellText[i]),
        ];
        IEnumerable<NTMFilterOption<ModContentPack?>> filterOptions = defs
            .Select(def => def.modContentPack)
            .OrderBy(mod => mod?.Name)
            .Select<ModContentPack?, NTMFilterOption<ModContentPack?>>(
                mod => mod == null ? new() : new(mod, mod.Name, null, mod.PackageIdPlayerFacing)
            );
        FilterOptions = [
            new OTMColumnFilterOption<ModContentPack?>(label, i => _cellValue[i], filterOptions),
        ];
    }

    public override bool AutoRefresh => false;

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Left;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    public override void DrawCell(Rect rect, int i)
    {
        _cellText[i].Draw(rect, GUIStyles.TableCell.String);

        if (Mouse.IsOver(rect))
        {
            rect.Tip(_cellValue[i]?.PackageIdPlayerFacing);
        }
    }

    protected override float GetCellWidth(int i)
    {
        return _cellWidth[i];
    }

    public override void Add(TRecord record)
    {
        ModContentPack? mod = record.Def.modContentPack;
        string text;
        float width;

        if (mod != null)
        {
            text = mod.Name;
            width = text.CalcSize(GUIStyles.TableCell.String).x;
        }
        else
        {
            text = "";
            width = 0f;
        }

        _cellValue.Add(mod);
        _cellText.Add(text);
        _cellWidth.Add(width);
    }

    public override void Refresh(int i, TRecord record)
    {
    }

    public override void Remove(int i)
    {
        _cellValue.ReplaceWithLast(i);
        _cellText.ReplaceWithLast(i);
        _cellWidth.ReplaceWithLast(i);
    }
}
