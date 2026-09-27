using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats.GUIScopes;

// Verse.Widgets.mouseOverScrollViewStack shenanigans are required so Mouse.IsOver()
// will return "true" only for visible parts of rects contained inside the scope.
// And the code is taken from Verse.Widgets.BeginScrollView.
internal readonly record struct GUIClipScope : IDisposable
{
    public GUIClipScope(Rect rect) : this(rect, Vector2.zero) { }

    public GUIClipScope(Rect rect, Vector2 scrollOffset)
    {
        Stack<bool> mouseOverScrollViewStack = Verse.Widgets.mouseOverScrollViewStack;
        bool mouseIsOverRect = rect.Contains(Event.current.mousePosition);

        if (mouseOverScrollViewStack.Count > 0)
        {
            bool flag = mouseOverScrollViewStack.Peek() && mouseIsOverRect;

            mouseOverScrollViewStack.Push(flag);
        }
        else
        {
            mouseOverScrollViewStack.Push(mouseIsOverRect);
        }

        GUI.BeginClip(rect, scrollOffset, Vector2.zero, false);
    }

    public void Dispose()
    {
        Verse.Widgets.mouseOverScrollViewStack.Pop();

        GUI.EndClip();
    }
}
