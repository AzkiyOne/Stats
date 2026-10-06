using System;
using Stats.Extensions;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Filters;

public sealed class BooleanFilter : Filter
{
    private readonly Func<int, bool> _getValue;

    public BooleanFilter(Func<int, bool> getValue)
    {
        _getValue = getValue;
    }

    public override bool IsActive => Value != null;

    public override event Action? OnChange;

    private bool? Value
    {
        get => field;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnChange?.Invoke();
        }
    } = null;

    public override Vector2 GetSize()
    {
        return new Vector2(Text.LineHeight * 2f, Text.LineHeight);
    }

    public override void Draw(Rect rect, Vector2 containerSize)
    {
        var origGUIColor = GUI.color;

        if (Value != true)
        {
            GUI.color = GUIStyles.Text.ColorSecondary;
        }

        if (rect.CutByX(rect.width / 2f).DrawButtonSubtle(Verse.Widgets.CheckboxOnTex))
        {
            Value = Value == true ? null : true;
        }

        GUI.color = origGUIColor;

        if (Value != false)
        {
            GUI.color = GUIStyles.Text.ColorSecondary;
        }

        if (rect.DrawButtonSubtle(Verse.Widgets.CheckboxOffTex))
        {
            Value = Value == false ? null : false;
        }

        GUI.color = origGUIColor;
    }

    public override bool Eval(int i)
    {
        return _getValue(i) == Value;
    }
}
