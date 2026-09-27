using UnityEngine;

namespace Stats.Widgets;

public sealed class HorContainer : Widget
{
    private readonly Widget[] _widgets;
    private readonly float _gap;

    public HorContainer(Widget[] widgets, float gap = 0f)
    {
        _widgets = widgets;
        _gap = gap;
        Vector2 size = Vector2.zero;
        int widgetsCount = widgets.Length;

        for (int i = 0; i < widgetsCount; i++)
        {
            Widget widget = widgets[i];
            Vector2 widgetSize = widget.Size;

            size.x += widgetSize.x;

            if (size.y < widgetSize.y)
            {
                size.y = widgetSize.y;
            }
        }

        size.x += (widgets.Length - 1) * gap;
        Size = size;
    }

    public override Vector2 Size { get; }

    public override void Draw(Rect rect)
    {
        float gap = _gap;
        int widgetsCount = _widgets.Length;
        ref Rect widgetRect = ref rect;

        for (int i = 0; i < widgetsCount; i++)
        {
            Widget widget = _widgets[i];
            Vector2 widgetSize = widget.Size;

            widgetRect.width = widgetSize.x;
            widgetRect.height = widgetSize.y;
            widget.Draw(widgetRect);
            widgetRect.x = widgetRect.xMax + gap;
        }
    }
}
