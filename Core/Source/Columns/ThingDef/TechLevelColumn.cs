using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets.Filters;
using UnityEngine;
using Verse;

namespace Stats.Columns.ThingDef;

public sealed class TechLevelColumn<TRecord> : Column<TRecord> where TRecord : IThingDefTableRecord
{
    private static readonly string[] _cellText;
    private static readonly float[] _cellWidth;

    static TechLevelColumn()
    {
        Array techLevels = Enum.GetValues(typeof(TechLevel));
        int techLevelsCount = techLevels.Length;

        _cellText = new string[techLevelsCount];
        _cellWidth = new float[techLevelsCount];

        foreach (TechLevel techLevel in techLevels)
        {
            byte i = (byte)techLevel;
            string text = techLevel.ToStringHuman().CapitalizeFirst();
            float width = text.CalcSize(GUIStyles.TableCell.String).x;

            _cellText[i] = text;
            _cellWidth[i] = width;
        }
    }

    private readonly List<TechLevel> _cellValue;

    public TechLevelColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records)
    {
        string label = def.LabelCap;
        int capacity = records.Capacity;
        _cellValue = new List<TechLevel>(capacity);
        SortOptions = [
            new ColumnSortOption<TechLevel>(label, i => _cellValue[i])
        ];
        IEnumerable<NTMFilterOption<TechLevel>> filterOptions = thingDefs
            .Select(thingDef => thingDef.techLevel)
            .Distinct()
            .OrderBy(techLevel => techLevel)
            .Select(techLevel => new NTMFilterOption<TechLevel>(techLevel, _cellText[(byte)techLevel]));
        FilterOptions = [
            new OTMColumnFilterOption<TechLevel>(label, i => _cellValue[i], filterOptions)
        ];
    }

    public override bool IsRefreshable => false;

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Left;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    public override void DrawCell(Rect rect, int i)
    {
        TechLevel techLevel = _cellValue[i];
        string text = _cellText[(byte)techLevel];

        text.Draw(rect, GUIStyles.TableCell.String);
    }

    protected override float GetCellWidth(int i)
    {
        TechLevel techLevel = _cellValue[i];

        return _cellWidth[(byte)techLevel];
    }

    public override void Add(TRecord record)
    {
        _cellValue.Add(record.ThingDef.techLevel);
    }

    public override void Refresh(int i, TRecord record)
    {
        _cellValue[i] = record.ThingDef.techLevel;
    }

    public override void Remove(int i)
    {
        _cellValue.ReplaceWithLast(i);
    }
}
