using RimWorld;
using Stats.Extensions;
using Stats.GUIScopes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using static Stats.GUIStyles.MainTabWindow;

namespace Stats.Widgets;

public sealed partial class MainTabWindow : RimWorld.MainTabWindow
{
    private readonly List<Tab> _tabs;
    private Tab? _activeTab;
    private readonly FloatMenu _tabDefsMenu;
    private static readonly TipSignal _openTabButtonTooltip = "Open tab";
    private Vector2 _tabListScrollPosition;
    private readonly float _defaultHeight;
    private float _maxYMin;
    private Action? _postDraw;

    public MainTabWindow()
    {
        _defaultHeight = base.RequestedTabSize.y;
        windowRect.height = _defaultHeight;
        List<TabDef> tabDefs = DefDatabase<TabDef>.AllDefsListForReading;
        int tabDefsCount = tabDefs.Count;
        List<FloatMenuOption> tabDefsMenuOptions = new(tabDefsCount);

        for (int i = 0; i < tabDefsCount; i++)
        {
            TabDef tabDef = tabDefs[i];
            FloatMenuOption menuOption = new(
                tabDef.LabelCap,
                () => OpenTab(tabDef),
                tabDef.Icon,
                tabDef.iconColor
            );
            tabDefsMenuOptions.Add(menuOption);
        }

        tabDefsMenuOptions.SortBy(option => option.Label);

        _tabDefsMenu = new FloatMenu(tabDefsMenuOptions);
        _tabs = new List<Tab>(tabDefsCount);
    }

    public override Vector2 RequestedTabSize => new(UI.screenWidth, windowRect.height);

    protected override float Margin => 1f;

    public override void DoWindowContents(Rect rect)
    {
        Event @event = Event.current;

        GUIUtils.PreOnGUI();

        // TODO: Remove this after you'll explixitly set word wrap for every inner widget.
        bool wordWrap = Text.WordWrap;
        Text.WordWrap = false;

        // Layout
        rect.CutLeft(out Rect toolbarRect, ToolbarWidth)
            .TakeRest(out Rect tabRect);

        toolbarRect.CutTop(out Rect openTabButtonRect, ToolbarWidth)
                   .TakeRest(out Rect tabListRect);

        tabRect.CutTop(out Rect resizeControlRect, GUIStyles.TableToolbar.Height);

        // Border
        if (@event.type == EventType.Repaint)
        {
            toolbarRect.DrawBorderRight(BorderColor);
        }

        // Buttons
        DrawOpenTabButton(openTabButtonRect);

        // Tab list
        // TODO:
        // - Add culling.
        Rect tabListContentRect = new(0f, 0f, ToolbarWidth, _tabs.Count * ToolbarWidth);
        Tab? draggedTab = GUIUtils.DraggedObj as Tab;

        using (new GUIScrollScope(tabListRect, ref _tabListScrollPosition, tabListContentRect, false))
        {
            Rect tabButtonRect = tabListContentRect with { height = ToolbarWidth };
            int tabsCount = _tabs.Count;

            for (int i = 0; i < tabsCount; i++)
            {
                Tab tab = _tabs[i];

                tab.DrawTitle(tabButtonRect, _activeTab == tab);

                if (draggedTab != null)
                {
                    HandleTabDrag(draggedTab, tab, tabButtonRect);
                }

                tabButtonRect.y = tabButtonRect.yMax;
            }
        }

        // Tab
        _activeTab?.DrawBody(tabRect);

        Text.WordWrap = wordWrap;

        DoResizeControl(resizeControlRect);

        if (_postDraw != null)
        {
            _postDraw();
            _postDraw = null;
        }

        GUIUtils.PostOnGUI();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void HandleTabDrag(Tab draggedTab, Tab tab, Rect tabRect)
    {
        float mouseY = Event.current.mousePosition.y;
        FloatRange verBoundaries = tabRect.VerBoundaries();

        if (verBoundaries.MinHalf().Includes(mouseY))
        {
            _postDraw ??= () => _tabs.MoveBeforeElem(draggedTab, tab);
        }
        else if (verBoundaries.MaxHalf().Includes(mouseY))
        {
            _postDraw ??= () => _tabs.MoveAfterElem(draggedTab, tab);
        }

        if (draggedTab == tab && Event.current.type == EventType.Repaint)
        {
            tabRect.HighlightActive();
        }
    }

    private void DrawOpenTabButton(Rect rect)
    {
        if (Event.current.type == EventType.Repaint)
        {
            rect.HighlightLight()
                .DrawBorderBottom(BorderColor)
                .ContractedBy(IconPadding)
                .DrawTextureFitted(TexButton.Plus)
                .Tip(_openTabButtonTooltip);
        }

        if (rect.DrawButtonGhostly())
        {
            _tabDefsMenu.Open();
        }
    }

    private void DoResizeControl(Rect rect)
    {
        Event curEvent = Event.current;
        GUIUtils.DragState dragState = rect.DoDraggableControl(this);

        if (dragState == GUIUtils.DragState.Dragged && curEvent.type == EventType.Layout)
        {
            float yMin = windowRect.yMin + GUIUtils.MouseDelta.y;
            windowRect.yMin = Mathf.Clamp(yMin, 0f, _maxYMin);
        }
        else if (curEvent is { type: EventType.MouseDown, clickCount: > 1 } && Mouse.IsOver(rect))
        {
            ResetSize();
            curEvent.Use();
        }
    }

    private void OpenTab(TabDef tabDef)
    {
        Tab tab = new(tabDef);

        _tabs.Insert(0, tab);
        SelectTab(tab);

        tab.OnTitleClick += SelectTab;
        tab.OnClose += CloseTab;
    }

    private void CloseTab(Tab tab)
    {
        int tabIndex = _tabs.IndexOf(tab);
        bool tabIsSelected = _activeTab == tab;

        if (tabIsSelected)
        {
            if (tabIndex > 0)
            {
                SelectTab(_tabs[tabIndex - 1]);
            }
            else if (_tabs.Count > 1)
            {
                SelectTab(_tabs[tabIndex + 1]);
            }
            else
            {
                _activeTab = null;
            }
        }

        _tabs.RemoveAt(tabIndex);
        tab.Dispose();
    }

    private void SelectTab(Tab tab)
    {
        _activeTab?.Suspend();
        _activeTab = tab;

        tab.Resume();
    }

    private void ResetSize()
    {
        windowRect.height = _defaultHeight;

        SetInitialSizeAndPosition();
    }

    public override void PostOpen()
    {
        _maxYMin = UI.screenHeight - MainButtonDef.ButtonHeight - GUIStyles.TableToolbar.Height;

        base.PostOpen();
        _activeTab?.Resume();
    }

    public override void PostClose()
    {
        _activeTab?.Suspend();
        GUIUtils.ResetDragState();

        base.PostClose();
    }
}
