using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.Widgets_Legacy;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets.Filters;

public abstract class FilterWithInputField<TLhs, TRhs> : Filter
{
    private const float OperatorButtonMinWidth = 24f;
    private const float OperatorButtonPaddingHor = GUIStyles.Global.PadXs;
    private const float InputFieldMinWidth = OperatorButtonMinWidth * 2f;

    private Vector2 _operatorButtonSize;
    private readonly FloatMenu _operatorsMenu;
    private readonly string _placeholder;
    private readonly Widgets_Legacy.Widget _clearButton;

    protected FilterWithInputField(IEnumerable<RelOperator<TLhs, TRhs>> operators, string? placeholder = null)
    {
        _placeholder = placeholder ?? "";

        var operatorsMenuOptions = new List<FloatMenuOption>(operators.Count());

        foreach (var @operator in operators)
        {
            var operatorString = @operator.Symbol.Colorize(GUIStyles.Text.ColorHighlight);
            var optionLabel = @operator.Description.Length > 0
                ? $"{operatorString} - {@operator.Description}"
                : operatorString;
            var option = new FloatMenuOption(optionLabel, () => Operator = @operator);
            operatorsMenuOptions.Add(option);
        }

        _operatorsMenu = new FloatMenu(operatorsMenuOptions);
        _clearButton = new Icon(TexButton.CloseXSmall, 0.5f)
        .HoverColor(GUIStyles.Text.ColorSecondary);
    }

    protected abstract string InputFieldText { get; }

    private bool InputFieldIsEmpty => InputFieldText.Length == 0;

    protected abstract RelOperator<TLhs, TRhs> Operator { get; set; }

    public sealed override Vector2 GetSize()
    {
        var size = _operatorButtonSize = CalcOperatorButtonSize();
        var inputFieldSize = CalcInputFieldSize();
        size.x += inputFieldSize.x;
        size.y = Mathf.Max(size.y, inputFieldSize.y);

        return size;
    }

    public sealed override void Draw(Rect rect, Vector2 _)
    {
        var origTextAnchor = Text.Anchor;
        var operatorButtonRect = rect.CutByX(_operatorButtonSize.x);
        var origGUIColor = GUI.color;

        if (IsActive == false)
        {
            GUI.color = GUIStyles.Text.ColorSecondary;
        }

        Text.Anchor = TextAnchor.LowerCenter;

        if (DrawOperatorButton(operatorButtonRect))
        {
            _operatorsMenu.Open();
        }

        Text.Anchor = TextAnchor.LowerLeft;
        var clearButtonRect = rect.RightPartPixels(_clearButton.GetSize().x);

        if
        (
            InputFieldIsEmpty == false
            && Event.current.type == EventType.MouseDown
            && Mouse.IsOver(clearButtonRect)
        )
        {
            ClearInputField();
            Event.current.Use();
        }

        DrawInputField(rect);

        if (InputFieldIsEmpty && IsActive == false)
        {
            rect.xMin += GUIStyles.Global.EstimatedInputFieldInnerPadding;
            Verse.Widgets.Label(rect, _placeholder);
        }
        else
        {
            MouseoverSounds.DoRegion(clearButtonRect);
            _clearButton.DrawIn(clearButtonRect);
        }

        Text.Anchor = origTextAnchor;
        GUI.color = origGUIColor;
    }

    private Vector2 CalcOperatorButtonSize()
    {
        var size = Text.CalcSize(Operator.Symbol);
        size.x += OperatorButtonPaddingHor * 2f;

        if (size.x < OperatorButtonMinWidth)
        {
            size.x = OperatorButtonMinWidth;
        }

        return size;
    }

    private bool DrawOperatorButton(Rect rect)
    {
        if (Operator.Description.Length > 0 && Mouse.IsOver(rect))
        {
            TooltipHandler.TipRegion(rect, Operator.Description);
        }

        return rect.DrawButtonSubtle(Operator.Symbol, GUI.color, OperatorButtonPaddingHor);
    }

    private Vector2 CalcInputFieldSize()
    {
        Vector2 size;

        if (InputFieldIsEmpty)
        {
            size = Text.CalcSize(_placeholder);
            size.x += GUIStyles.Global.EstimatedInputFieldInnerPadding * 2f;
        }
        else
        {
            size = Text.CalcSize(InputFieldText);
            size.x += GUIStyles.Global.Pad + _clearButton.GetSize().x;
        }

        if (size.x < InputFieldMinWidth)
        {
            size.x = InputFieldMinWidth;
        }

        return size;
    }

    protected abstract void DrawInputField(Rect rect);

    protected abstract void ClearInputField();
}
