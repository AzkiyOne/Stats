using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats;

public static class GUIUtils
{
    private static readonly GUIContent _guiContent = new();
    private static Vector2 _prevMousePos;
    private static Vector2 _mouseDelta;

    internal static Vector2 MouseDelta => _mouseDelta;

    internal static void PreOnGUI()
    {
        Vector2 mousePos = UI.MousePositionOnUIInverted;
        _mouseDelta = (mousePos - _prevMousePos).Rounded();
        _prevMousePos = mousePos;
    }

    internal static void PostOnGUI()
    {
        // Fix drag state (if needed)
        if (_draggedObj != null && Input.GetMouseButtonUp(0))
        {
            ResetDragState();
        }
    }

    [Obsolete]
    internal static float Opacity { get; set; } = 1f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete]
    internal static Color AdjustedForGUIOpacity(this Color color)
    {
        color.a *= Opacity;

        return color;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 CalcSize(this string text, GUIStyle style)
    {
        _guiContent.text = text.StripTags();

        return style.CalcSize(_guiContent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect DrawLabel(this Rect rect, string text, GUIStyle style)
    {
        GUI.Label(rect, text, style);

        return rect;
    }

    public static void Draw(this string text, Rect rect, GUIStyle style) => rect.DrawLabel(text, style);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect DrawTexture(this Rect rect, Texture2D texture)
    {
        return rect.DrawTexture(texture, Color.white);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect DrawTexture(this Rect rect, Texture2D texture, Color color, ScaleMode scaleMode = ScaleMode.StretchToFill)
    {
        GUI.DrawTexture(
            rect,
            texture,
            scaleMode,
            true,
            0f,
            color,
            Vector2.zero,
            Vector2.zero
        );

        return rect;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect DrawTextureFitted(this Rect rect, Texture2D texture)
    {
        return rect.DrawTexture(texture, Color.white, ScaleMode.ScaleToFit);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect DrawTextureFitted(this Rect rect, Texture2D texture, float scale)
    {
        return rect.DrawTextureFitted(texture, Color.white, scale);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect DrawTextureFitted(this Rect rect, Texture2D texture, Color color, float scale)
    {
        Rect scaledRect;

        if (scale != 1f)
        {
            scaledRect = rect.ScaledBy(scale);
        }
        else
        {
            scaledRect = rect;
        }

        scaledRect.DrawTexture(texture, color, ScaleMode.ScaleToFit);

        return rect;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect DrawBorderTop(this Rect rect, Color color)
    {
        (rect with { height = 1f }).Fill(color);

        return rect;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect DrawBorderBottom(this Rect rect, Color color)
    {
        (rect with { y = Mathf.Round(rect.yMax) - 1f, height = 1f }).Fill(color);

        return rect;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect DrawBorderRight(this Rect rect, Color color)
    {
        (rect with { x = Mathf.Round(rect.xMax) - 1f, width = 1f }).Fill(color);

        return rect;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect Fill(this Rect rect, Color color)
    {
        return rect.DrawTexture(BaseContent.WhiteTex, color);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect Highlight(this Rect rect)
    {
        return rect.DrawTexture(TexUI.HighlightTex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect HighlightLight(this Rect rect)
    {
        return rect.DrawTexture(Verse.Widgets.LightHighlight);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect HighlightSelected(this Rect rect)
    {
        return rect.DrawTexture(TexUI.HighlightSelectedTex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect HighlightActive(this Rect rect)
    {
        return rect.Fill(GUIStyles.Global.HighlightActiveColor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect Tip(this Rect rect, TipSignal tip)
    {
        TooltipHandler.TipRegion(rect, tip);

        return rect;
    }

    internal static Rect DrawButtonGhostlySkinSilent(this Rect rect)
    {
        if (Mouse.IsOver(rect))
        {
            rect.Highlight();
        }

        return rect;
    }

    internal static Rect DrawButtonGhostlySkin(this Rect rect)
    {
        MouseoverSounds.DoRegion(rect);
        rect.DrawButtonGhostlySkinSilent();

        return rect;
    }

    internal static bool DrawButtonGhostly(this Rect rect)
    {
        if (Event.current.type == EventType.Repaint)
        {
            DrawButtonGhostlySkin(rect);
        }

        return rect.DrawButtonEmpty();
    }

    internal static bool DrawButtonSubtle(this Rect rect, string text, Color textColor, float padHor = 0f)
    {
        bool mouseIsOverRect = Mouse.IsOver(rect);
        Color origGUIColor = GUI.color;

        if (mouseIsOverRect)
        {
            GUI.color = GenUI.MouseoverColor;
        }

        bool wasClicked = Verse.Widgets.ButtonInvisible(rect);

        Verse.Widgets.DrawAtlas(rect, Verse.Widgets.ButtonSubtleAtlas);

        rect.x += padHor;
        rect.width -= padHor * 2f;
        if (mouseIsOverRect)
        {
            rect.x += GUIStyles.Global.ButtonSubtleContentHoverOffset;
            rect.y -= GUIStyles.Global.ButtonSubtleContentHoverOffset;
        }

        GUI.color = textColor;
        Verse.Widgets.Label(rect, text);

        GUI.color = origGUIColor;

        return wasClicked;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool DrawButtonSubtle(this Rect rect, string text, float padHor = 0f)
    {
        return rect.DrawButtonSubtle(text, Color.white, padHor);
    }

    internal static bool DrawButtonSubtle(this Rect rect, Texture2D texture, float textureScale = 0.7f)
    {
        bool mouseIsOverRect = Mouse.IsOver(rect);
        Color origGUIColor = GUI.color;

        if (mouseIsOverRect)
        {
            GUI.color = GenUI.MouseoverColor;
        }

        bool wasClicked = Verse.Widgets.ButtonInvisible(rect);

        Verse.Widgets.DrawAtlas(rect, Verse.Widgets.ButtonSubtleAtlas);

        if (mouseIsOverRect)
        {
            rect.x += GUIStyles.Global.ButtonSubtleContentHoverOffset;
            rect.y -= GUIStyles.Global.ButtonSubtleContentHoverOffset;
        }

        Verse.Widgets.DrawTextureFitted(rect, texture, textureScale);

        GUI.color = origGUIColor;

        return wasClicked;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool DrawButtonEmpty(this Rect rect)
    {
        return GUI.Button(rect, GUIContent.none, GUIStyle.none);
    }

    // Why object?
    //
    // I just need an unique id that is:
    // - Not order based.
    // - Not position based.
    //
    // Because order and/or position of a control may change, while it is being dragged around.
    // 
    // Object is just simplest way to implement this.
    // Tho we have to be extra careful not to cause a memory leak.
    private static object? _draggedObj;
    internal static object? DraggedObj => _dragStarted ? _draggedObj : null;
    private static bool _dragStarted;
    private static Vector3 _dragMouseDownPos;
    private const float MinDragStartDistanceSquared = 3f * 3f;

    // If you need standard button behaviour, use a button or just check for events manually.
    internal static DragState DraggableControl(this Rect rect, object obj)
    {
        if (_draggedObj == null)
        {
            if (Input.GetMouseButtonDown(0) && Mouse.IsOver(rect))
            {
                _draggedObj = obj;
                _dragMouseDownPos = Input.mousePosition;
            }
        }
        else if (_draggedObj == obj)
        {
            if (_dragStarted)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    ResetDragState();

                    return DragState.DragEnded;
                }

                return DragState.Dragged;
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    ResetDragState();
                }
                else if ((_dragMouseDownPos - Input.mousePosition).sqrMagnitude > MinDragStartDistanceSquared)
                {
                    _dragStarted = true;

                    return DragState.DragStarted;
                }
            }
        }

        return DragState.Idle;
    }

    internal static void ResetDragState()
    {
        _draggedObj = null;
        _dragStarted = false;
    }

    internal enum DragState
    {
        Idle,
        DragStarted,
        Dragged,
        DragEnded,
    }
}
