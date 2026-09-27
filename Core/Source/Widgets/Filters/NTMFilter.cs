using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Stats.Extensions;
using Stats.Widgets_Legacy;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets.Filters;

public abstract class NTMFilter<TValue, TOption> : Filter
{
    private const float ButtonMinWidth = 24f;
    private const float ButtonPadHor = GUIStyles.Global.PadSm;

    private readonly RelOperator<TValue, HashSet<TOption>> _defaultOperator;
    private readonly List<TValue> _values;
    private readonly IEnumerable<RelOperator<TValue, HashSet<TOption>>> _operators;
    private readonly string _buttonTextWhenInactive;
    private readonly HashSet<TOption> _selectedOptions = [];
    private RelOperator<TValue, HashSet<TOption>> _operator;
    // TODO: See if IEnumerable is most fitting type here.
    private readonly IEnumerable<NTMFilterOption<TOption>> _options;
    private string? _info;

    protected NTMFilter(
        List<TValue> values,
        IEnumerable<NTMFilterOption<TOption>> options,
        IEnumerable<RelOperator<TValue, HashSet<TOption>>> operators,
        RelOperator<TValue, HashSet<TOption>> defaultOperator,
        string? label = null
    )
    {
        _values = values;
        _operator = _defaultOperator = defaultOperator;
        _options = options;
        _buttonTextWhenInactive = label ?? "...";
        _operators = operators;
    }

    public override bool IsActive => _selectedOptions.Count > 0;

    private string ButtonText => IsActive ? Info : _buttonTextWhenInactive;

    private List<NTMFilterOption<TOption>> OptionsList => field ??= _options.ToList();

    private OptionsWindowWidget OptionsWindow => field ??= new(OptionsList, this, _operators);

    private string Info
    {
        get
        {
            if (_info != null)
            {
                return _info;
            }

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendInNewLine($"{Operator.Symbol.Colorize(GUIStyles.Text.ColorHighlight)} - {Operator.Description}:");

            foreach (var option in OptionsList)
            {
                if (_selectedOptions.Contains(option.Value))
                {
                    stringBuilder.AppendInNewLine($"- {option.Label}");
                }
            }

            return _info = stringBuilder.ToString();
        }
    }

    protected RelOperator<TValue, HashSet<TOption>> Operator
    {
        get => _operator;
        set
        {
            if (_operator == value)
            {
                return;
            }

            _operator = value;
            _info = null;
            Resize();
            OnChange?.Invoke();
        }
    }

    public override event Action? OnChange;

    public override Vector2 GetSize()
    {
        var size = Text.CalcSize(ButtonText);
        size.x += ButtonPadHor * 2f;

        if (size.x < ButtonMinWidth)
        {
            size.x = ButtonMinWidth;
        }

        return size;
    }

    public override void Draw(Rect rect, Vector2 containerSize)
    {
        var origGUIColor = GUI.color;

        if (IsActive == false)
        {
            GUI.color = GUIStyles.Text.ColorSecondary;
        }

        if (rect.DrawButtonSubtle(ButtonText, GUI.color, ButtonPadHor))
        {
            OptionsWindow.Open();
        }

        GUI.color = origGUIColor;
    }

    public override bool Eval(int i)
    {
        return Operator.Eval(_values[i], _selectedOptions);
    }

    public sealed override void Reset()
    {
        _operator = _defaultOperator;
        Clear();
    }

    private void Clear()
    {
        _selectedOptions.Clear();
        _info = null;
        Resize();
        OnChange?.Invoke();
    }

    private void HandleOptionClick(TOption option)
    {
        if (_selectedOptions.Contains(option))
        {
            _selectedOptions.Remove(option);
        }
        else
        {
            if (Event.current.control == false)
            {
                _selectedOptions.Clear();
            }

            _selectedOptions.Add(option);
        }

        _info = null;
        Resize();
        OnChange?.Invoke();
    }

    public override void NotifyChanged()
    {
        OnChange?.Invoke();
    }

    private sealed class OptionsWindowWidget : Window
    {
        private const float OptionHoverHorShiftAmount = 4f;
        private const float OptionPadHor = GUIStyles.Global.Pad;
        private const float OptionPadVer = GUIStyles.Global.PadXs;
        private const float OperatorButtonSize = 28f;

        private static readonly Color _borderColor = Verse.Widgets.SeparatorLineColor;
        private static readonly Color _backgroundColor = Verse.Widgets.WindowBGFillColor;
        private static readonly Color _optionHoverBackgroundColor = FloatMenuOption.ColorBGActiveMouseover;
        private static readonly float _optionWidgetHeight = Text.LineHeight + OptionPadVer * 2f;

        private readonly Widgets_Legacy.Widget _optionsList;
        private readonly Vector2 _optionsListSize;
        private readonly Widgets_Legacy.Widget _toolbar;
        private readonly Vector2 _toolbarSize;
        private bool _willScrollHor = false;
        private Vector2 _scrollPosition;

        public OptionsWindowWidget(
            List<NTMFilterOption<TOption>> options,
            NTMFilter<TValue, TOption> parent,
            IEnumerable<RelOperator<TValue, HashSet<TOption>>> operators
        )
        {
            doWindowBackground = false;
            drawShadow = false;
            closeOnClickedOutside = true;
            _toolbar = new Widgets_Legacy.VerticalContainer([
                new Widgets_Legacy.HorizontalContainer([
                    ..operators.Select(@operator =>
                        new Widgets_Legacy.Label(@operator.Symbol)
                        .TextAnchor(TextAnchor.MiddleCenter)
                        .SizeAbs(OperatorButtonSize)
                        .Color(GUIStyles.Text.ColorHighlight)
                        .SkipNextExtension(() => parent.Operator != @operator)
                        .HoverShift(GUIStyles.Global.ButtonSubtleContentHoverOffset, -GUIStyles.Global.ButtonSubtleContentHoverOffset)
                        .BackgroundAtlas(Verse.Widgets.ButtonSubtleAtlas)
                        .HoverColor(GenUI.MouseoverColor)
                        .Color(GUIStyles.Text.ColorSecondary)
                        .SkipNextExtension(() => parent.Operator != @operator)
                        .OnClick(() => parent.Operator = @operator)
                        .Tooltip(@operator.Description)
                    ),

                    new Widgets_Legacy.Label("Clear")
                    .TextAnchor(TextAnchor.MiddleLeft)
                    .PaddingAbs(GUIStyles.Global.PadSm, 0f)
                    .WidthIncRel(1f)
                    .HeightAbs(OperatorButtonSize)
                    .ToButtonSubtle(parent.Clear),
                ], shareFreeSpace: true)
                .WidthRel(1f),

                new Widgets_Legacy.Label("<i>Hold [Ctrl] to select multiple options.</i>")
                .PaddingAbs(GUIStyles.Global.PadSm, 0f)
                .BorderLeft(_borderColor)
                .BorderRight(_borderColor)
                .WidthRel(1f),
            ]);
            _toolbarSize = _toolbar.GetSize();

            var optionsListMaxHeight = UI.screenHeight * 0.6f - _toolbarSize.y;
            var columns = new List<List<Widgets_Legacy.Widget>>()
            {
                new()
            };
            var currentColumn = columns[0];
            var currentColumnHeight = 0f;

            foreach (var option in options)
            {
                Widgets_Legacy.Widget optionWidget = option
                .ToWidget()
                .PaddingAbs(OptionPadHor, OptionPadVer)
                .WidthRel(1f)
                .HoverShiftHor(OptionHoverHorShiftAmount)
                .Background(rect =>
                {
                    if (parent._selectedOptions.Contains(option.Value))
                    {
                        Verse.Widgets.DrawHighlightSelected(rect);
                    }
                })
                .HoverBackground(_optionHoverBackgroundColor)
                .OnClick(() => parent.HandleOptionClick(option.Value));

                if (option.Tooltip?.Length > 0)
                {
                    optionWidget = optionWidget.Tooltip(option.Tooltip);
                }

                var optionWidgetHeight = optionWidget.GetSize().y;
                currentColumnHeight += optionWidgetHeight;

                if (currentColumnHeight > optionsListMaxHeight)
                {
                    currentColumn = [];
                    currentColumnHeight = optionWidgetHeight;
                    columns.Add(currentColumn);
                }

                if (currentColumn.Count > 0)
                {
                    optionWidget = optionWidget.PaddingAbs(0f, 0f, 1f, 0f);
                }

                currentColumn.Add(optionWidget);
            }

            var columnWidgets = new List<Widgets_Legacy.Widget>(columns.Count);

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                Widgets_Legacy.Widget columnWidget = new Widgets_Legacy.VerticalContainer(column);

                if (i < columns.Count - 1)
                {
                    columnWidget = columnWidget.BorderRight(_borderColor);
                }

                columnWidgets.Add(columnWidget);
            }

            _optionsList = new HorizontalContainer(columnWidgets, stretchItems: true)
            .WidthRel(1f);
            _optionsListSize = _optionsList.GetSize();
        }

        protected override float Margin => 0f;

        public override void DoWindowContents(Rect rect)
        {
            var origGUIOpacity = GUIUtils.Opacity;
            var origGUIColor = GUI.color;

            DoFadeEffect(rect);

            Verse.Widgets.DrawBoxSolid(rect, _backgroundColor.AdjustedForGUIOpacity());

            var rectSize = rect.size;

            _toolbar.Draw(rect.CutByY(_toolbar.GetSize().y), rectSize);

            var borderColor = _borderColor.AdjustedForGUIOpacity();
            if (Event.current.type == EventType.Repaint)
            {
                // Hor:
                // - Top
                var horRect = rect with { height = 1f };
                Verse.Widgets.DrawBoxSolid(horRect, borderColor);
                // - Bottom
                horRect.y = rect.yMax - 1f;
                Verse.Widgets.DrawBoxSolid(horRect, borderColor);
                // Ver:
                // - Left
                var verRect = rect with { width = 1f };
                Verse.Widgets.DrawBoxSolid(verRect, borderColor);
                // - Right
                verRect.x = rect.xMax - 1f;
                Verse.Widgets.DrawBoxSolid(verRect, borderColor);
            }

            rect = rect.ContractedBy(1f);

            if (_willScrollHor)
            {
                var viewRect = new Rect(0f, 0f, _optionsListSize.x, _optionsListSize.y);

                if (Event.current.type == EventType.ScrollWheel && Mouse.IsOver(rect))
                {
                    _scrollPosition.x = Mathf.Max(_scrollPosition.x + Event.current.delta.y * 20f, 0f);
                    Event.current.Use();
                }

                Verse.Widgets.BeginScrollView(rect, ref _scrollPosition, viewRect);

                _optionsList.Draw(viewRect, rectSize);

                Verse.Widgets.EndScrollView();

                Verse.Widgets.DrawLineHorizontal(rect.x, rect.yMax - GenUI.ScrollBarWidth - 1f, rect.width, borderColor);
            }
            else
            {
                _optionsList.Draw(rect, rectSize);
            }

            DrawRowSeparators(rect, borderColor);

            GUIUtils.Opacity = origGUIOpacity;
            GUI.color = origGUIColor;
        }

        private void DoFadeEffect(Rect rect)
        {
            rect = rect.ContractedBy(-5f);

            const float maxAllovedMouseDistFromRect = 95f;

            if (rect.Contains(Event.current.mousePosition) == false)
            {
                var mouseDistFromRect = GenUI.DistFromRect(rect, Event.current.mousePosition);

                GUIUtils.Opacity = 1f - mouseDistFromRect / maxAllovedMouseDistFromRect;
                GUI.color = GUI.color.AdjustedForGUIOpacity();

                if (mouseDistFromRect > maxAllovedMouseDistFromRect)
                {
                    Close();
                }
            }
        }

        public override void Close(bool doCloseSound = false)
        {
            SoundDefOf.FloatMenu_Cancel.PlayOneShotOnCamera();
            base.Close(doCloseSound);
        }

        protected override void SetInitialSizeAndPosition()
        {
            var position = UI.MousePositionOnUIInverted;
            var size = new Vector2(
                Mathf.Max(_toolbarSize.x, _optionsListSize.x + 2f),
                _toolbarSize.y + _optionsListSize.y + 2f
            );
            var maxWidth = UI.screenWidth * 0.9f;

            if (size.x > maxWidth)
            {
                _willScrollHor = true;
                size.x = maxWidth;
                size.y += GenUI.ScrollBarWidth + 1f;
            }

            if (position.x + size.x > UI.screenWidth)
            {
                position.x = UI.screenWidth - size.x - GUIStyles.Global.Pad;
            }

            if (position.y + size.y > UI.screenHeight)
            {
                position.y = UI.screenHeight - size.y - GUIStyles.Global.Pad;
            }

            windowRect = new Rect(position, size);
        }

        private static void DrawRowSeparators(Rect rect, Color color)
        {
            var y = rect.y + _optionWidgetHeight;

            if (Event.current.type != EventType.Repaint) return;

            while (y < rect.yMax)
            {
                Verse.Widgets.DrawLineHorizontal(rect.x, y, rect.width, color);
                y += _optionWidgetHeight + 1f;
            }
        }
    }
}

public readonly record struct NTMFilterOption<TValue>
{
    public NTMFilterOption()
    {
        Value = default;
        Label = "<i>Undefined</i>";
    }

    public NTMFilterOption(TValue value, string label, Widgets_Legacy.Widget? icon = null, string? tooltip = null)
    {
        Value = value;
        Label = label;
        Icon = icon;
        Tooltip = tooltip;
    }

    public TValue Value { get; }

    public string Label { get; }

    public Widgets_Legacy.Widget? Icon { get; }

    public string? Tooltip { get; }

    public Widgets_Legacy.Widget ToWidget()
    {
        var label = new Widgets_Legacy.Label(Label);

        if (Icon != null)
        {
            return new Widgets_Legacy.HorizontalContainer([Icon, label], GUIStyles.Global.PadSm);
        }

        return label;
    }
}
