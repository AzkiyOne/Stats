using UnityEngine;
using Verse;

namespace Stats.Widgets_Legacy.Extensions;

public sealed class HoverForegroundWidgetExtension : WidgetExtension
{
    private readonly Texture2D Texture;
    private readonly Color Color;
    internal HoverForegroundWidgetExtension(Widget widget, Texture2D texture, Color color) : base(widget)
    {
        Texture = texture;
        Color = color;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        Widget.Draw(rect, containerSize);

        if (Event.current.type == EventType.Repaint && Mouse.IsOver(rect))
        {
            GUI.DrawTexture(
                rect, Texture, ScaleMode.StretchToFill, true, 0f, Color.AdjustedForGUIOpacity(), 0f, 0f
            );
        }
    }
}
