using RimWorld;
using Stats.Extensions;
using Stats.NumberFormats;
using Stats.TableRecords;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Columns.BuildableDef;

public class StatColumn<TRecord> : Column<TRecord> where TRecord : IBuildableDefTableRecord
{
    private static readonly bool _autoRefresh = typeof(IThingTableRecord).IsAssignableFrom(typeof(TRecord));

    private readonly StatDef _statDef;
    private readonly List<decimal> _cellValue;
    private readonly List<string> _cellText;
    private readonly List<float> _cellWidth;
    private readonly List<Lazy<TipSignal>?> _cellTooltip;
    private readonly NumberFormat _numberFormat;

    public StatColumn(StatColumnDef def, List<TRecord> records, object _) : base(def, records, _autoRefresh)
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

        if (_autoRefresh == false && _numberFormat is IObservable observable)
        {
            observable.OnChange += RefreshOnce;
        }
    }

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Right;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    public override void DrawCell(Rect rect, int i)
    {
        _cellText[i].Draw(rect, GUIStyles.TableCell.Number);

        Lazy<TipSignal>? tooltip = _cellTooltip[i];

        if (tooltip != null)
        {
            rect.Tip(tooltip.Value);
        }
    }

    protected override float GetCellWidth(int i)
    {
        return _cellWidth[i];
    }

    protected virtual StatRequest GetStatRequest(TRecord record)
    {
        return record.StatRequest;
    }

    private void GetCellDrawData(decimal value, float statValue, StatRequest statRequest, out string text, out Lazy<TipSignal>? tooltip, out float width)
    {
        if (value != 0m)
        {
            text = _numberFormat.FormatNumber(value);
            width = text.CalcSize(GUIStyles.TableCell.Number).x;
            tooltip = new Lazy<TipSignal>(() => _statDef.Worker.GetExplanationFull(statRequest, ToStringNumberSense.Absolute, statValue));

            return;
        }

        text = "";
        width = 0f;
        tooltip = null;
    }

    private void GetCellData(TRecord record, out decimal value, out float statValue, out StatRequest statRequest)
    {
        statRequest = GetStatRequest(record);

        if (_statDef.Worker.ShouldShowFor(statRequest))
        {
            statValue = _statDef.Worker.GetValue(statRequest);
            value = _numberFormat.ToDecimal(statValue);

            return;
        }

        value = 0m;
        statValue = 0f;
    }

    public override void Add(TRecord record)
    {
        GetCellData(record, out decimal value, out float statValue, out StatRequest statRequest);
        GetCellDrawData(value, statValue, statRequest, out string text, out Lazy<TipSignal>? tooltip, out float width);

        _cellValue.Add(value);
        _cellText.Add(text);
        _cellWidth.Add(width);
        _cellTooltip.Add(tooltip);
    }

    public override void RefreshCell(int i, TRecord record)
    {
        GetCellData(record, out decimal value, out float statValue, out StatRequest statRequest);

        if (_cellValue[i] != value)
        {
            GetCellDrawData(value, statValue, statRequest, out string text, out Lazy<TipSignal>? tooltip, out float width);

            _cellValue[i] = value;
            _cellText[i] = text;
            _cellWidth[i] = width;
            _cellTooltip[i] = tooltip;
        }
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

        if (_numberFormat is IObservable observable)
        {
            observable.OnChange -= RefreshOnce;
        }
    }
}
