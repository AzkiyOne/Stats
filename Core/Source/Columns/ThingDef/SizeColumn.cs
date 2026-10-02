using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets.Filters;
using UnityEngine;
using Verse;

namespace Stats.Columns.ThingDef;

public sealed class SizeColumn<TRecord> : Column<TRecord> where TRecord : IThingDefTableRecord
{
    private readonly List<decimal> _cellValue;
    private readonly List<string> _cellText;
    private readonly List<float> _cellWidth;

    public SizeColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records)
    {
        string label = def.LabelCap;
        int capacity = records.Capacity;
        _cellValue = new List<decimal>(capacity);
        _cellText = new List<string>(capacity);
        _cellWidth = new List<float>(capacity);
        SortOptions = [
            new ColumnSortOption<decimal>(label, i => _cellValue[i])
        ];
        IEnumerable<NTMFilterOption<decimal>> filterOptions = thingDefs
            .Select(thingDef => NormalizeSize(thingDef.size))
            .Distinct()
            .OrderBy(size => size.Area)
            .Select(size => new NTMFilterOption<decimal>(size.Area, size.ToStringCross()));
        FilterOptions = [
            new OTMColumnFilterOption<decimal>(label, i => _cellValue[i], filterOptions)
        ];
    }

    public override bool IsRefreshable => false;

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Right;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    private static IntVec2 NormalizeSize(IntVec2 vec2)
    {
        // Because 4x5 == 5x4.
        return new IntVec2(Math.Max(vec2.x, vec2.z), Math.Min(vec2.x, vec2.z));
    }

    public override void DrawCell(Rect rect, int i)
    {
        _cellText[i].Draw(rect, GUIStyles.TableCell.Number);
    }

    protected override float GetCellWidth(int i)
    {
        return _cellWidth[i];
    }

    public override void Add(TRecord record)
    {
        IntVec2 size = record.ThingDef.size;
        int area = size.Area;
        string text;
        float width;

        if (area != 0m)
        {
            text = NormalizeSize(size).ToStringCross();
            width = text.CalcSize(GUIStyles.TableCell.Number).x;
        }
        else
        {
            text = "";
            width = 0f;
        }

        _cellValue.Add(area);
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
