using System.Collections.Generic;
using UnityEngine;

namespace Stats.Widgets_Legacy;

// See vertical variant for comments.
public sealed class HorizontalContainer : Widget
{
    private readonly List<Widget> Children;
    private readonly float Gap;
    private readonly float TotalGapAmount;
    private readonly bool ShareFreeSpace;
    private readonly bool StretchItems;
    private float OccupiedSpaceAmount;
    public HorizontalContainer(
        List<Widget> children,
        float gap = 0f,
        bool shareFreeSpace = false,
        bool stretchItems = false
    )
    {
        Children = children;
        Gap = gap;
        TotalGapAmount = (Children.Count - 1) * Gap;
        ShareFreeSpace = shareFreeSpace;
        StretchItems = stretchItems;

        foreach (var child in children)
        {
            child.Parent = this;
        }
    }
    public override Vector2 GetSize()
    {
        Vector2 size;
        size.x = TotalGapAmount;
        size.y = 0f;

        if (ShareFreeSpace || StretchItems)
        {
            OccupiedSpaceAmount = TotalGapAmount;
        }

        foreach (var child in Children)
        {
            var childSize = child.GetSize();

            size.x += childSize.x;
            size.y = Mathf.Max(size.y, childSize.y);

            if (ShareFreeSpace || StretchItems)
            {
                OccupiedSpaceAmount += child.GetFixedSize().x;
            }
        }

        return size;
    }
    public override void Draw(Rect rect, Vector2 _)
    {
        var size = rect.size;
        size.x = Mathf.Max(size.x - OccupiedSpaceAmount, 0f);
        var additionalChildWidth = 0f;

        if (StretchItems)
        {
            additionalChildWidth = Mathf.Round(size.x / Children.Count);
        }

        foreach (var child in Children)
        {
            rect.size = child.GetSize(size);
            rect.width += additionalChildWidth;

            child.Draw(rect, size);

            rect.x = rect.xMax + Gap;
        }
    }
}
