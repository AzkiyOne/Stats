using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets.Filters;
using UnityEngine;
using Verse;

namespace Stats.Columns.ThingDef;

public sealed class SizeColumn<TRecord> : Column<TRecord, IntVec2> where TRecord : IThingDefTableRecord
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

    protected override IntVec2 GetValueFromRecord(TRecord record)
    {
        return NormalizeSize(record.ThingDef.size);
    }

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

    private void GetCellValues(IntVec2 size, out decimal area, out string text, out float width)
    {
        area = size.Area;

        if (area != 0m)
        {
            text = size.ToStringCross();
            width = text.CalcSize(GUIStyles.TableCell.Number).x;
        }
        else
        {
            text = "";
            width = 0f;
        }
    }

    protected override void AddValue(IntVec2 value)
    {
        GetCellValues(value, out decimal area, out string text, out float width);

        _cellValue.Add(area);
        _cellText.Add(text);
        _cellWidth.Add(width);
    }

    protected override void SetValue(int i, IntVec2 value)
    {
        GetCellValues(value, out decimal area, out string text, out float width);

        _cellValue[i] = area;
        _cellText[i] = text;
        _cellWidth[i] = width;
    }

    protected override void RemoveValue(int i)
    {
        _cellValue.ReplaceWithLast(i);
        _cellText.ReplaceWithLast(i);
        _cellWidth.ReplaceWithLast(i);
    }
}
