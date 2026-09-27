using System;
using UnityEngine;

namespace Stats.GUIScopes;

internal readonly record struct GUIScrollScope : IDisposable
{
    public GUIScrollScope(Rect rect, ref Vector2 scrollPosition, Rect viewRect, bool showScrollbars = true)
    {
        Verse.Widgets.BeginScrollView(rect, ref scrollPosition, viewRect, showScrollbars);
    }

    public void Dispose() => Verse.Widgets.EndScrollView();
}
