using Stats.Extensions;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed class ThingDefIconInteractive : ThingDefIcon
{
    private readonly ThingDef _thingDef;
    private readonly ThingDef? _stuffDef;
    private TipSignal? _tooltip;

    public ThingDefIconInteractive(ThingDef thingDef, ThingDef? stuffDef = null) : base(thingDef, stuffDef)
    {
        _thingDef = thingDef;
        _stuffDef = stuffDef;
    }

    public ThingDefIconInteractive(Vector2 size, ThingDef thingDef, ThingDef? stuffDef = null) : base(size, thingDef, stuffDef)
    {
        _thingDef = thingDef;
        _stuffDef = stuffDef;
    }

    private TipSignal Tooltip => _tooltip ??=
        _stuffDef == null
            ? _thingDef.LabelCap.RawText
            : $"{_stuffDef.LabelAsStuff.CapitalizeFirst()} {_thingDef.label}";

    public override void Draw(Rect rect)
    {
        base.Draw(rect);

        bool iconWasClicked = rect.DrawButtonGhostly();

        if (iconWasClicked)
        {
            _thingDef.OpenInfoDialog(_stuffDef);
        }

        if (Mouse.IsOver(rect))
        {
            TooltipHandler.TipRegion(rect, Tooltip);
        }
    }
}
