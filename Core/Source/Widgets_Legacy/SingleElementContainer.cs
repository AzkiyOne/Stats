using UnityEngine;

namespace Stats.Widgets_Legacy;

public sealed class SingleElementContainer : Widget
{
    private readonly Widget Widget;
    private readonly float OccupiedWidth = 0f;
    private readonly float OccupiedHeight = 0f;
    public SingleElementContainer(Widget widget)
    {
        Widget = widget;
        widget.Parent = this;

        var widgetSize = widget.GetFixedSize();

        OccupiedWidth = widgetSize.x;
        OccupiedHeight = widgetSize.y;
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize();
    }
    public override void Draw(Rect rect, Vector2 _)
    {
        var size = rect.size;
        size.x = Mathf.Max(size.x - OccupiedWidth, 0f);
        size.y = Mathf.Max(size.y - OccupiedHeight, 0f);

        rect.size = Widget.GetSize(size);
        Widget.Draw(rect, size);
    }
}
