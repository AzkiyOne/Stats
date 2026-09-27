using UnityEngine;

namespace Stats.Widgets;

public sealed class InlineIcon : Widget
{
    private readonly Texture2D _texture;
    private readonly Color _color;
    private readonly float _scale;

    public InlineIcon(Texture2D texture, Color color, float scale)
    {
        _texture = texture;
        float height = GUIStyles.Text.LineHeight;
        float width = height * (texture.width / (float)texture.height);
        Size = new Vector2(width, height);
        _color = color;
        _scale = scale;
    }

    public override Vector2 Size { get; }

    public override void Draw(Rect rect)
    {
        rect.DrawTextureFitted(_texture, _color, _scale);
    }
}
