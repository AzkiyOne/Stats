using UnityEngine;
using Verse;

namespace Stats.Widgets;

public class ThingDefIcon : Widget
{
    private readonly Texture2D _texture;
    private readonly Color _color;
    private readonly Vector2 _proportions;
    private readonly Rect _coords;
    private readonly float _scale;
    private readonly float _angle;
    private readonly Vector2 _offset;

    public ThingDefIcon(ThingDef thingDef, ThingDef? stuffDef = null)
        : this(new(GUIStyles.Text.LineHeight, GUIStyles.Text.LineHeight), thingDef, stuffDef) { }

    public ThingDefIcon(Vector2 size, ThingDef thingDef, ThingDef? stuffDef = null)
    {
        _texture = Verse.Widgets.GetIconFor(thingDef, stuffDef) ?? BaseContent.BadTex;
        _scale = GenUI.IconDrawScale(thingDef);
        _angle = thingDef.uiIconAngle;
        _offset = thingDef.uiIconOffset;
        Size = size;

        if (stuffDef != null)
        {
            _color = thingDef.GetColorForStuff(stuffDef);
        }
        else
        {
            _color = thingDef.uiIconColor;
        }

        if (thingDef.graphicData != null)
        {
            _proportions = thingDef.graphicData.drawSize.RotatedBy(thingDef.defaultPlacingRot);

            if (thingDef.uiIconPath.NullOrEmpty() && thingDef.graphicData.linkFlags != 0)
            {
                _coords = new Rect(0f, 0.5f, 0.25f, 0.25f);// Verse.Widgets.LinkedTexCoords
            }
            else
            {
                _coords = new Rect(0f, 0f, 1f, 1f);// Verse.Widgets.DefaultTexCoords
            }
        }
        else
        {
            _proportions = new Vector2(_texture.width, _texture.height);
        }
    }

    public override Vector2 Size { get; }

    public override void Draw(Rect rect)
    {
        Color color = GUI.color;
        GUI.color = _color;

        rect.position += _offset * rect.size;
        Verse.Widgets.DrawTextureFitted(rect, _texture, _scale, _proportions, _coords, _angle);

        GUI.color = color;
    }
}
