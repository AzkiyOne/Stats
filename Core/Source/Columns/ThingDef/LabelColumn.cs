using System.Collections.Generic;
using Stats.Columns;
using Stats.TableRecords;
using Stats.Widgets;
using Stats.Widgets.Filters;
using Verse;

namespace Stats.Columns.ThingDef;

// Note to myself: Don't remove stuff label. It's important because
// modded stuffs may have the same color as vanilla ones or other modded stuffs.
// Replacing label with icon won't do, because ex. all of the leathers have the same
// icon but of different color.
public sealed class LabelColumn<TRecord> : ThingDefColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public LabelColumn(LabelColumnDef def) : base(def)
    {
        ThingDefOptions = def.ThingDefOptions;
    }

    protected override IEnumerable<Verse.ThingDef?> ThingDefOptions { get; }

    // TODO: Add stuff (material) filter. Remember that it should only appear when it makes sense.
    public override ICollection<ColumnFilterOption> FilterOptions => [
        new ColumnFilterOption("Label", () => new StringFilter(i => this[i].Text ?? "")),
        ..base.FilterOptions
    ];

    protected override ThingDefColumnCell MakeCell(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        Verse.ThingDef? stuffDef = record.StatRequest.StuffDef;
        string text = stuffDef == null
            ? thingDef.LabelCap.RawText
            : $"{stuffDef.LabelAsStuff.CapitalizeFirst()} {thingDef.label}";
        Widget icon = new ThingDefIconInteractive(thingDef, stuffDef);

        return new ThingDefColumnCell(thingDef, text, icon);
    }

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

    /*
    
    TODO: 
    
    Instead of having this "filter", have two sets of tables for thing defs that can be made from stuff:
    - A table that lists every def + stuff variant. Default columns are ones whose values depend on stuff.
    - A table that lists bases (without default stuff). Default columns are ones whose values do not depend on stuff.

    Note:

    This being a filter is a hack. It doesn't work in "OR" mode and is semantically incorrect.
    
    There are 2 ways of fixing this issue.

    The easy way is to introduce some special/pre filters that would be displayed in window's toolbar 
    and be applied separatedly.

    The hard way is to allow for grouping table's rows by a column.
    The issue that is being solved by this filter is "show me only values that do not depend on stuff".
    Grouping can be implemented by implementing "Equals" on a cell.
    
    */
    //private sealed class StuffedVariantsDisplayModeToggleButton : FilterWidget
    //{
    //    private bool _IsActive = false;
    //    public override bool IsActive => _IsActive;
    //    public override event Action? OnChange;
    //    private Texture2D Texture => _IsActive ? Verse.Widgets.CheckboxOnTex : Verse.Widgets.CheckboxOffTex;
    //    private static readonly TipSignal Manual =
    //        "Click to show only distinct item material variants.\n\n" +
    //        "Material for each distinct variant is chosen based on item's type definition.";
    //    public StuffedVariantsDisplayModeToggleButton()
    //    {
    //    }
    //    protected override Vector2 GetSize()
    //    {
    //        return new Vector2(Text.LineHeight, Text.LineHeight);
    //    }
    //    public override void Draw(Rect rect, Vector2 _)
    //    {
    //        var origTextAnchor = Text.Anchor;
    //        Text.Anchor = TextAnchor.LowerLeft;

    //        var origGUIColor = GUI.color;
    //        if (_IsActive == false)
    //        {
    //            GUI.color = Globals.GUI.TextColorSecondary;
    //        }

    //        if (Widgets.Draw.ButtonImageSubtle(rect, Texture))
    //        {
    //            _IsActive = !_IsActive;

    //            OnChange?.Invoke();
    //        }

    //        Text.Anchor = origTextAnchor;
    //        GUI.color = origGUIColor;

    //        TooltipHandler.TipRegion(rect, Manual);
    //    }
    //    public override bool Eval(ObjectTableWidget.Cell cell)
    //    {
    //        if (_IsActive)
    //        {
    //            // Do not filter out stuffless things.
    //            // - You can filter them out with stuff filter.
    //            // - There are cases where one may want to compare
    //            //   things by stats unrelated to stuff. Ex. equipped
    //            //   stat offsets.
    //            return ((Cell)cell).IsMadeFromDefaultStuff;
    //        }

    //        return true;
    //    }
    //    public override void Reset()
    //    {
    //    }
    //    public override void NotifyChanged()
    //    {
    //        OnChange?.Invoke();
    //    }
    //}
}
