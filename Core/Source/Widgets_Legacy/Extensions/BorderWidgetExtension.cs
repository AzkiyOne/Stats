using UnityEngine;

namespace Stats.Widgets_Legacy.Extensions;

public sealed class BorderWidgetExtension : WidgetExtension
{
    private readonly float Thickness;
    private readonly Color Color;
    internal BorderWidgetExtension(Widget widget, float thickness, Color color) : base(widget)
    {
        Thickness = thickness;
        Color = color;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        var size = base.GetSize(containerSize);
        size.x += Thickness * 2;
        size.y += Thickness * 2;

        return size;
    }
    public override Vector2 GetSize()
    {
        var size = base.GetSize();
        size.x += Thickness * 2;
        size.y += Thickness * 2;

        return size;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            var color = Color.AdjustedForGUIOpacity();
            // Hor:
            // - Top
            var horRect = rect with { height = Thickness };
            Verse.Widgets.DrawBoxSolid(horRect, color);
            // - Bottom
            horRect.y = rect.yMax - Thickness;
            Verse.Widgets.DrawBoxSolid(horRect, color);
            // Ver:
            // - Left
            var verRect = rect with { width = Thickness };
            Verse.Widgets.DrawBoxSolid(verRect, color);
            // - Right
            verRect.x = rect.xMax - Thickness;
            Verse.Widgets.DrawBoxSolid(verRect, color);
        }

        rect.x += Thickness;
        rect.y += Thickness;
        rect.width -= Thickness * 2;
        rect.height -= Thickness * 2;

        Widget.Draw(rect, containerSize);
    }
}
