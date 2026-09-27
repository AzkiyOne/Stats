using UnityEngine;

namespace Stats.Widgets;

public abstract class TabBodyWidget
{
    public abstract void Draw(Rect rect);

    public abstract void Focus();

    public abstract void Unfocus();

    public abstract void Dispose();
}
