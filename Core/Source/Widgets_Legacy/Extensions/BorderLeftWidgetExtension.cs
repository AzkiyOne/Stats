using UnityEngine;

namespace Stats.Widgets_Legacy.Extensions;

public sealed class BorderLeftWidgetExtension : WidgetExtension
{
    private readonly float Thickness;
    private readonly Color Color;
    internal BorderLeftWidgetExtension(Widget widget, float thickness, Color color) : base(widget)
    {
        Thickness = thickness;
        Color = color;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        var size = base.GetSize(containerSize);
        size.x += Thickness;

        return size;
    }
    public override Vector2 GetSize()
    {
        var size = base.GetSize();
        size.x += Thickness;

        return size;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Verse.Widgets.DrawBoxSolid(
                rect with { width = Thickness },
                Color.AdjustedForGUIOpacity()
            );
        }

        rect.x += Thickness;
        rect.width -= Thickness;

        Widget.Draw(rect, containerSize);
    }
}
