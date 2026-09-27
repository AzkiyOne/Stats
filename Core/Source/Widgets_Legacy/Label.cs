using UnityEngine;

namespace Stats.Widgets_Legacy;

public sealed class Label : Widget
{
    public string Text
    {
        get => _text;
        set
        {
            if (value == _text)
            {
                return;
            }

            _text = value;

            Resize();
        }
    }

    private string _text;

    public Label(string text)
    {
        _text = text;
    }

    public static readonly GUIStyle Style;

    static Label()
    {
        Style = new GUIStyle(Verse.Text.fontStyles[1]);
        Style.alignment = TextAnchor.MiddleLeft;
        Style.wordWrap = false;
    }

    public override Vector2 GetSize()
    {
        return Verse.Text.CalcSize(Text);
    }

    public override void Draw(Rect rect, Vector2 _)
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        rect.DrawLabel(Text, Style);
    }
}
