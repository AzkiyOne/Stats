using UnityEngine;

namespace Stats.Widgets;

public abstract class Widget : IDrawable
{
    public abstract Vector2 Size { get; }

    public abstract void Draw(Rect rect);
}
