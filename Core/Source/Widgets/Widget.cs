using UnityEngine;

namespace Stats.Widgets;

public abstract class Widget
{
    public abstract Vector2 Size { get; }

    public abstract void Draw(Rect rect);
}
