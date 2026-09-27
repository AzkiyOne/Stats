using UnityEngine;

namespace Stats.Widgets_Legacy.Extensions;

public sealed class BackgroundWidgetExtension : WidgetExtension
{
    private readonly Texture2D Texture;
    private readonly Color Color;
    internal BackgroundWidgetExtension(Widget widget, Texture2D texture, Color color) : base(widget)
    {
        Texture = texture;
        Color = color;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            GUI.DrawTexture(
                rect, Texture, ScaleMode.StretchToFill, true, 0f, Color.AdjustedForGUIOpacity(), 0f, 0f
            );
        }

        Widget.Draw(rect, containerSize);
    }
}
