using UnityEngine;

namespace Stats.Widgets;

public abstract class TabBodyWidget
{
    public abstract void Draw(Rect rect);

    public abstract void Resume();

    public abstract void Suspend();

    public abstract void Dispose();
}
