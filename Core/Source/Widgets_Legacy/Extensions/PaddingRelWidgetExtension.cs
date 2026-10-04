using UnityEngine;

namespace Stats.Widgets_Legacy.Extensions;

public sealed class PaddingRelWidgetExtension : WidgetExtension
{
    private readonly float Left;
    private readonly float Top;
    private readonly float Horizontal;
    private readonly float Vertical;
    internal PaddingRelWidgetExtension(
        Widget widget,
        float left,
        float right,
        float top,
        float bottom
    ) : base(widget)
    {
        Left = left;
        Top = top;
        Horizontal = left + right;
        Vertical = top + bottom;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);
        size.x += Horizontal * containerSize.x;
        size.y += Vertical * containerSize.y;

        return size;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        rect.x += Left * containerSize.x;
        rect.y += Top * containerSize.y;
        rect.width -= Horizontal * containerSize.x;
        rect.height -= Vertical * containerSize.y;

        Widget.Draw(rect, containerSize);
    }
}
