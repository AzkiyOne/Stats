using System;
using System.Collections.Generic;
using Stats.Extensions;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public abstract class Tab
{
    internal event Action<Tab>? OnTitleClick;
    internal event Action<Tab>? OnClose;

    private readonly TipSignal _tooltip;
    private readonly Texture2D _icon;
    private readonly Color _iconColor;
    private readonly float _iconScale;
    private readonly FloatMenu _menu;

    protected Tab(TabDef def)
    {
        _tooltip = def.LabelCap;
        if (def.description?.Length > 0)
        {
            _tooltip += $"\n\n{def.description}";
        }
        _icon = def.Icon;
        _iconColor = def.iconColor;
        _iconScale = def.iconScale;
        List<FloatMenuOption> menuOptions = [
            new FloatMenuOption("Close", () => OnClose?.Invoke(this))
        ];
        _menu = new FloatMenu(menuOptions);
    }

    internal void DrawTitle(Rect rect, DragManager<Tab> dragManager, bool isSelected)
    {
        Event @event = Event.current;

        if (@event.type == EventType.Repaint)
        {
            if (dragManager.IsDragged(this))
            {
                rect.HighlightActive();
            }
            else if (isSelected)
            {
                rect.HighlightSelected();
            }

            rect.Tip(_tooltip)
                .ContractedBy(GUIStyles.MainTabWindow.IconPadding)
                .DrawTextureFitted(_icon, _iconColor, _iconScale);
        }

        if (@event is { type: EventType.MouseUp, modifiers: EventModifiers.None } && Mouse.IsOver(rect))
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

        dragManager.OnGUI(rect, this);

        rect.DrawButtonGhostly();
    }

    protected abstract TabBodyWidget Widget { get; }

    internal void DrawBody(Rect rect) => Widget.Draw(rect);

    internal void Focus() => Widget.Focus();

    internal void Unfocus() => Widget.Unfocus();

    internal void Dispose() => Widget.Dispose();
}
