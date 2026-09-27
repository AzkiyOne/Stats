using System;
using UnityEngine;
using Verse;

namespace Stats;

internal abstract class DragManager<T> where T : class
{
    private T? _draggedWidget;

    public event Action<T, T>? OnDragBefore;
    public event Action<T, T>? OnDragAfter;

    private void StartDrag(T widget)
    {
        _draggedWidget = widget;
    }

    public void EndDrag()
    {
        _draggedWidget = null;
        GUIUtils.ReleaseMouseControl();
        //Event.current.Use();
    }

    public bool IsDragged(T widget)
    {
        return _draggedWidget == widget;
    }

    protected abstract bool IsMouseBeforeMiddle(Rect rect, Vector2 mousePosition);

    protected abstract bool IsMouseAfterMiddle(Rect rect, Vector2 mousePosition);

    public void OnGUI(Rect rect, T widget) => OnGUI(rect, rect, widget);

    public void OnGUI(Rect grabRect, Rect dropRect, T widget)
    {
        Event @event = Event.current;

        if (@event is { type: EventType.MouseDown, button: 0, modifiers: EventModifiers.None } && Mouse.IsOver(grabRect))
        {
            StartDrag(widget);
        }
        else if (_draggedWidget != null)
        {
            if (OriginalEventUtility.EventType == EventType.MouseDrag)
            {
                Vector2 mousePosition = @event.mousePosition;

                if (IsMouseBeforeMiddle(dropRect, mousePosition))
                {
                    OnDragBefore?.Invoke(_draggedWidget, widget);
                }
                else if (IsMouseAfterMiddle(dropRect, mousePosition))
                {
                    OnDragAfter?.Invoke(_draggedWidget, widget);
                }

                @event.Use();
            }
            else if (@event.rawType == EventType.MouseUp)
            {
                EndDrag();
            }
        }
    }
}

internal sealed class HorDragManager<T> : DragManager<T> where T : class
{
    protected override bool IsMouseBeforeMiddle(Rect rect, Vector2 mousePosition)
    {
        float mouseX = mousePosition.x;
        float xMiddle = rect.x + rect.width / 2f;

        return mouseX > rect.x && mouseX < xMiddle;
    }

    protected override bool IsMouseAfterMiddle(Rect rect, Vector2 mousePosition)
    {
        float mouseX = mousePosition.x;
        float xMiddle = rect.x + rect.width / 2f;

        return mouseX > xMiddle && mouseX < rect.xMax;
    }
}

internal sealed class VerDragManager<T> : DragManager<T> where T : class
{
    protected override bool IsMouseBeforeMiddle(Rect rect, Vector2 mousePosition)
    {
        float mouseY = mousePosition.y;
        float yMiddle = rect.y + rect.height / 2f;

        return mouseY > rect.y && mouseY < yMiddle;
    }

    protected override bool IsMouseAfterMiddle(Rect rect, Vector2 mousePosition)
    {
        float mouseY = mousePosition.y;
        float yMiddle = rect.y + rect.height / 2f;

        return mouseY > yMiddle && mouseY < rect.yMax;
    }
}
