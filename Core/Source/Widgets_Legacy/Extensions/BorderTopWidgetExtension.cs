using UnityEngine;

namespace Stats.Widgets_Legacy.Extensions;

public sealed class BorderTopWidgetExtension : WidgetExtension
{
    private readonly float Thickness;
    private readonly Color Color;
    internal BorderTopWidgetExtension(Widget widget, float thickness, Color color) : base(widget)
    {
        Thickness = thickness;
        Color = color;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        var size = base.GetSize(containerSize);
        size.y += Thickness;

        return size;
    }
    public override Vector2 GetSize()
    {
        var size = base.GetSize();
        size.y += Thickness;

        return size;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Verse.Widgets.DrawBoxSolid(
                rect with { height = Thickness },
                Color.AdjustedForGUIOpacity()
            );
        }

        rect.y += Thickness;
        rect.height -= Thickness;

        Widget.Draw(rect, containerSize);
    }
}
