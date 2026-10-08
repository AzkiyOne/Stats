using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets;
using Stats.Widgets.Filters;
using UnityEngine;
using Verse;

namespace Stats.Columns.ThingDef;

// Note to myself: Don't remove stuff label. It's important because
// modded stuffs may have the same color as vanilla ones or other modded stuffs.
// Replacing label with icon won't do, because ex. all of the leathers have the same
// icon but of different color.
public sealed class LabelColumn<TRecord> : Column<TRecord> where TRecord : IThingDefTableRecord
{
    private static readonly HashSet<Verse.ThingDef> _nullSet = [null];

    private readonly List<Verse.ThingDef> _cellThingDef;
    private readonly List<Verse.ThingDef?> _cellStuffDef;
    private readonly List<string> _cellText;
    private readonly List<Widget> _cellIcon;
    private readonly List<float> _cellWidth;

    public LabelColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records)
    {
        string label = def.LabelCap;
        int capacity = records.Capacity;
        _cellThingDef = new List<Verse.ThingDef>(capacity);
        _cellStuffDef = new List<Verse.ThingDef?>(capacity);
        _cellText = new List<string>(capacity);
        _cellIcon = new List<Widget>(capacity);
        _cellWidth = new List<float>(capacity);
        SortOptions = [
            new ColumnSortOption<string>("Label", i => _cellText[i])
        ];
        IEnumerable<NTMFilterOption<Verse.ThingDef>> thingDefFilterOptions = thingDefs
            .OrderBy(thingDef => thingDef.label)
            .Select<Verse.ThingDef, NTMFilterOption<Verse.ThingDef>>(
                thingDef => new(thingDef, thingDef.LabelCap, new Widgets_Legacy.ThingDefIcon(thingDef))
            );
        IEnumerable<NTMFilterOption<Verse.ThingDef?>> stuffDefFilterOptions = thingDefs
            .SelectMany(thingDef => thingDef.GetAllowedStuffs() ?? _nullSet)
            .Distinct()
            .OrderBy(thingDef => thingDef?.label)
            .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
                thingDef => thingDef == null ? new() : new(thingDef, thingDef.LabelCap, new Widgets_Legacy.ThingDefIcon(thingDef))
            );
        FilterOptions = [
            new OTMColumnFilterOption<Verse.ThingDef>("Type", i => _cellThingDef[i], thingDefFilterOptions),
            new OTMColumnFilterOption<Verse.ThingDef?>("Material", i => _cellStuffDef[i], stuffDefFilterOptions),
            new StringColumnFilterOption("Label", i => _cellText[i]),
        ];
    }

    public override bool AutoRefresh => false;

    public override ColumnContentAlignment ContentAlignment => ColumnContentAlignment.Left;

    public override ICollection<ColumnSortOption> SortOptions { get; }

    public override ICollection<ColumnFilterOption> FilterOptions { get; }

    public override void DrawCell(Rect rect, int i)
    {
        string text = _cellText[i];
        Widget icon = _cellIcon[i];

        rect.ContractedBy(GUIStyles.TableCell.PadLR, GUIStyles.TableCell.PadTB)
            .CutLeft(out Rect iconRect, icon.Size.x)
            .CutLeft(GUIStyles.TableCell.ContentSpacing)
            .TakeRest(out Rect labelRect);

        icon.Draw(iconRect);

        if (Event.current.type == EventType.Repaint)
        {
            text.Draw(labelRect, GUIStyles.TableCell.StringNoPad);
        }
    }

    protected override float GetCellWidth(int i)
    {
        return _cellWidth[i];
    }

    public override void Add(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        Verse.ThingDef? stuffDef = record.StatRequest.StuffDef;
        string text = stuffDef == null
            ? thingDef.LabelCap.RawText
            : $"{stuffDef.LabelAsStuff.CapitalizeFirst()} {thingDef.label}";
        Widget icon = new ThingDefIconInteractive(thingDef, stuffDef);
        float textWidth = text.CalcSize(GUIStyles.TableCell.StringNoPad).x;
        float width = icon.Size.x + GUIStyles.TableCell.ContentSpacing + textWidth + GUIStyles.TableCell.PadHor;

        _cellThingDef.Add(thingDef);
        _cellStuffDef.Add(stuffDef);
        _cellText.Add(text);
        _cellIcon.Add(icon);
        _cellWidth.Add(width);
    }

    public override void Refresh(int i, TRecord record)
    {
    }

    public override void Swap(int i1, int i2)
    {
        _cellThingDef.Swap(i1, i2);
        _cellStuffDef.Swap(i1, i2);
        _cellText.Swap(i1, i2);
        _cellIcon.Swap(i1, i2);
        _cellWidth.Swap(i1, i2);
    }

    public override void Remove(int i)
    {
        _cellThingDef.ReplaceWithLast(i);
        _cellStuffDef.ReplaceWithLast(i);
        _cellText.ReplaceWithLast(i);
        _cellIcon.ReplaceWithLast(i);
        _cellWidth.ReplaceWithLast(i);
    }

    //protected override ThingDefColumnCell MakeCell(TRecord record)
    //{
    //    Verse.ThingDef thingDef = record.ThingDef;
    //    Verse.ThingDef? stuffDef = record.StatRequest.StuffDef;
    //    string text = stuffDef == null
    //        ? thingDef.LabelCap.RawText
    //        : $"{stuffDef.LabelAsStuff.CapitalizeFirst()} {thingDef.label}";
    //    Widget icon = new ThingDefIconInteractive(thingDef, stuffDef);

    //    return new ThingDefColumnCell(thingDef, text, icon);
    //}

    // TODO: Make a separate "Researched" column.
    //IEnumerable<ObjectTableWidget.ColumnPart> IColumnWorker<VirtualThing>.GetObjectProps()
    //{
    //    var filterWidget_Researched = new BooleanFilter(
    //        cell => ((Cell)cell).Def.GetResearchProjectDefs()?.All(researchProjectDef => researchProjectDef.IsFinished) is true or null
    //    );
    //    Globals.Events.OnResearchCompleted += () =>
    //    {
    //        if (filterWidget_Researched.IsActive)
    //        {
    //            filterWidget_Researched.NotifyChanged();
    //        }
    //    };
    //    yield return new(new Label("Researched"), filterWidget_Researched);

    //    if (contextObjects.Any(@object => @object.StuffDef != null))
    //    {
    //        yield return new(new Label("Distinct"), new StuffedVariantsDisplayModeToggleButton());

    //        var stuffFilterOptions = contextObjects
    //            .Select(@object => @object.StuffDef)
    //            .Distinct()
    //            .OrderBy(thingDef => thingDef?.label)
    //            .Select<ThingDef?, NTMFilterOption<ThingDef?>>(
    //                thingDef => thingDef == null
    //                    ? new()
    //                    : new(thingDef, thingDef.LabelCap, new ThingDefIcon(thingDef))
    //            );
    //        var stuffFilter = new OTMFilter<ThingDef?>(cell => ((Cell)cell).StuffDef, stuffFilterOptions);
    //        yield return new(new Label("Material"), stuffFilter);
    //    }
    //}
}
