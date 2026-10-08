using System;
using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.NumberFormats;
using Stats.TableRecords;
using UnityEngine;
using Verse;

namespace Stats.Columns.BuildableDef;

public class StatColumn<TRecord> : Column<TRecord> where TRecord : IBuildableDefTableRecord
{
    private readonly StatDef _statDef;
    private readonly List<decimal> _cellValue;
    private readonly List<string> _cellText;
    private readonly List<float> _cellWidth;
    private readonly List<Lazy<TipSignal>?> _cellTooltip;
    private readonly NumberFormat _numberFormat;

    public StatColumn(StatColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
        string label = def.LabelCap;
        int capacity = records.Capacity;
        _statDef = def.stat;
        _numberFormat = def.NumberFormat;
        _cellValue = new List<decimal>(capacity);
        _cellText = new List<string>(capacity);
        _cellWidth = new List<float>(capacity);
        _cellTooltip = new List<Lazy<TipSignal>?>(capacity);
        SortOptions = [
            new ColumnSortOption<decimal>(label, i => _cellValue[i])
        ];
        FilterOptions = [
            new NumberColumnFilterOption(label, i => _cellValue[i])
        ];

        if (typeof(TRecord) is IThingTableRecord)
        {
            AutoRefresh = true;
        }
        // Elif because a refreshable column will get refreshed naturally anyway.
        else if (_numberFormat is IDynamicNumberFormat dynamicNumberFormat)
        {
            dynamicNumberFormat.OnChange += RefreshOnce;
        }
    }

    public override bool AutoRefresh { get; }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Right;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    public override void DrawCell(Rect rect, int i)
    {
        _cellText[i].Draw(rect, GUIStyles.TableCell.Number);
    }

    protected override float GetCellWidth(int i)
    {
        return _cellWidth[i];
    }

    protected virtual StatRequest GetStatRequest(TRecord record)
    {
        return record.StatRequest;
    }

    private void GetCellValues(TRecord record, out decimal value, out string text, out Lazy<TipSignal>? tooltip, out float width)
    {
        StatRequest statRequest = GetStatRequest(record);

        if (_statDef.Worker.ShouldShowFor(statRequest))
        {
            float statValue = _statDef.Worker.GetValue(statRequest);
            value = _numberFormat.ToDecimal(statValue);

            if (value != 0m)
            {
                text = _numberFormat.FormatNumber(value);
                width = text.CalcSize(GUIStyles.TableCell.Number).x;
                tooltip = new Lazy<TipSignal>(() => _statDef.Worker.GetExplanationFull(statRequest, ToStringNumberSense.Absolute, statValue));

                return;
            }
        }

        value = 0m;
        text = "";
        width = 0f;
        tooltip = null;
    }

    public override void Add(TRecord record)
    {
        GetCellValues(record, out decimal value, out string text, out Lazy<TipSignal>? tooltip, out float width);

        _cellValue.Add(value);
        _cellText.Add(text);
        _cellWidth.Add(width);
        _cellTooltip.Add(tooltip);
    }

    public override void Refresh(int i, TRecord record)
    {
        GetCellValues(record, out decimal value, out string text, out Lazy<TipSignal>? tooltip, out float width);

        _cellValue[i] = value;
        _cellText[i] = text;
        _cellWidth[i] = width;
        _cellTooltip[i] = tooltip;
    }

    public override void Swap(int i1, int i2)
    {
        _cellValue.Swap(i1, i2);
        _cellText.Swap(i1, i2);
        _cellWidth.Swap(i1, i2);
        _cellTooltip.Swap(i1, i2);
    }

    public override void Replace(int i1, int i2)
    {
        _cellValue.Replace(i1, i2);
        _cellText.Replace(i1, i2);
        _cellWidth.Replace(i1, i2);
        _cellTooltip.Replace(i1, i2);
    }

    public override void Dispose()
    {
        base.Dispose();

        if (_numberFormat is IDynamicNumberFormat dynamicNumberFormat)
        {
            dynamicNumberFormat.OnChange -= RefreshOnce;
        }
    }
}
