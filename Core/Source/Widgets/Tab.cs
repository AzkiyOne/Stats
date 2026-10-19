using Stats.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed class Tab
{
    private readonly TipSignal _tooltip;
    private readonly Texture2D _icon;
    private readonly Color _iconColor;
    private readonly float _iconScale;
    private readonly FloatMenu _menu;
    private readonly IDrawable _widget;

    public Tab(TabDef def) : this(def.Worker.Widget, def.LabelCap, def.description, def.Icon, def.iconColor, def.iconScale)
    {
    }

    public Tab(IDrawable widget, string label, string? description, Texture2D icon, Color iconColor, float iconScale)
    {
        _widget = widget;
        _tooltip = label;
        if (description?.Length > 0)
        {
            _tooltip += $"\n\n{description}";
        }
        _icon = icon;
        _iconColor = iconColor;
        _iconScale = iconScale;
        List<FloatMenuOption> menuOptions = [
            new FloatMenuOption("Close", () => OnClose?.Invoke(this))
        ];
        _menu = new FloatMenu(menuOptions);
    }

    internal event Action<Tab>? OnTitleClick;

    internal event Action<Tab>? OnClose;

    internal void DrawTitle(Rect rect, bool isSelected)
    {
        Event @event = Event.current;

        if (@event.type == EventType.Repaint)
        {
            if (isSelected)
            {
                rect.HighlightSelected();
            }

            rect.Tip(_tooltip)
                .ContractedBy(GUIStyles.MainTabWindow.IconPadding)
                .DrawTextureFitted(_icon, _iconColor, _iconScale);
        }

        GUIUtils.DragState dragState = rect.DoDraggableControl(this);
        bool wasClicked = rect.DrawButtonGhostly();

        if (wasClicked && dragState == GUIUtils.DragState.Idle)
        {
            if (@event.button == 0)
            {
                OnTitleClick?.Invoke(this);
            }
            else if (@event.button == 1)
            {
                _menu.Open();
            }
        }
    }

    internal void DrawBody(Rect rect) => _widget.Draw(rect);

    internal void Resume() => (_widget as ISuspendable)?.Resume();

    internal void Suspend() => (_widget as ISuspendable)?.Suspend();

    internal void Dispose() => (_widget as IDisposable)?.Dispose();
}
