using UnityEngine;
using Verse;

namespace Stats.Widgets_Legacy;

// Designed for rendering textures along with text.
public class InlineTexture : Widget
{
    public Texture2D Texture { get; set; }
    private readonly float Scale;
    public InlineTexture(Texture2D texture, float scale = 1f)
    {
        Texture = texture;
        Scale = scale;
    }
    public override Vector2 GetSize()
    {
        var width = Text.LineHeight * (Texture.width / (float)Texture.height);

        return new Vector2(width, Text.LineHeight);
    }
    public override void Draw(Rect rect, Vector2 _)
    {
        if (Event.current.type == EventType.Repaint)
        {
            rect.DrawTextureFitted(Texture, GUI.color, Scale);
        }
    }
}
