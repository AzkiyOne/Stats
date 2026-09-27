using UnityEngine;

namespace Stats.Widgets_Legacy.Extensions;

public sealed class ColorWidgetExtension : WidgetExtension
{
    public Color Color { get; set; }
    internal ColorWidgetExtension(Widget widget, Color color) : base(widget)
    {
        Color = color;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        var origGUIColor = GUI.color;
        GUI.color = Color.AdjustedForGUIOpacity();

        Widget.Draw(rect, containerSize);

        GUI.color = origGUIColor;
    }
}
