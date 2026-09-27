using UnityEngine;

namespace Stats.Widgets;

public sealed class Label : Widget
{
    private readonly string _text;
    private readonly GUIStyle _style;

    // TODO: Pass style explicitly at call sites and remove this constructor.
    public Label(string text) : this(text, GUIStyles.TableCell.StringNoPad) { }

    public Label(string text, GUIStyle style)
    {
        _text = text;
        _style = style;
        Size = text.CalcSize(style);
    }

    public override Vector2 Size { get; }

    public override void Draw(Rect rect)
    {
        rect.DrawLabel(_text, _style);
    }
}
