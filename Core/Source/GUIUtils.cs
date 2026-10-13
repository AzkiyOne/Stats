using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats;

public static class GUIUtils
{

    private static readonly GUIContent _guiContent = new();

    // TODO:
    // Investigate whether we need this.
    // And if we do, there is UnityEngine.GUI._buttonDownActive, which i assume does the same thing.
    internal static void ReleaseMouseControl() {}

    internal static float Opacity { get; set; } = 1f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete]
    internal static Color AdjustedForGUIOpacity(this Color color)
    {
        color.a *= Opacity;

        return color;
    }

    public static Vector2 CalcSize(this string text, GUIStyle style)
    {
        _guiContent.text = text.StripTags();
        return style.CalcSize(_guiContent);
    }

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

    internal static Rect DrawBorderTop(this Rect rect, Color color)
    {
        (rect with { height = 1f }).Fill(color);

        return rect;
    }

    internal static Rect DrawBorderBottom(this Rect rect, Color color)
    {
        (rect with { y = Mathf.Round(rect.yMax) - 1f, height = 1f }).Fill(color);

        return rect;
    }

    internal static Rect DrawBorderRight(this Rect rect, Color color)
    {
        (rect with { x = Mathf.Round(rect.xMax) - 1f, width = 1f }).Fill(color);

        return rect;
    }

    internal static Rect Fill(this Rect rect, Color color)
    {
        return rect.DrawTexture(BaseContent.WhiteTex, color);
    }

    internal static Rect Highlight(this Rect rect)
    {
        return rect.DrawTexture(TexUI.HighlightTex);
    }

    internal static Rect HighlightLight(this Rect rect)
    {
        return rect.DrawTexture(Verse.Widgets.LightHighlight);
    }

    internal static Rect HighlightSelected(this Rect rect)
    {
        return rect.DrawTexture(TexUI.HighlightSelectedTex);
    }

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

    internal static bool DrawButtonGhostly(this Rect rect)
    {
        MouseoverSounds.DoRegion(rect);

        if (Event.current.type == EventType.Repaint && Mouse.IsOver(rect))
        {
            rect.Highlight();
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

    internal static bool DrawButtonEmpty(this Rect rect)
    {
        return GUI.Button(rect, GUIContent.none, GUIStyle.none);
    }
}
