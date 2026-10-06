using UnityEngine;
using Verse;

namespace Stats;

public static class GUIStyles
{
    private static readonly GUIStyle _baseStyle = new(Text.FontMedium)
    {
        wordWrap = false
    };

    internal static class Global
    {
        internal const float Pad = 10f;
        internal const float PadSm = 5f;
        internal const float PadXs = 3f;
        internal const float EstimatedInputFieldInnerPadding = 2f;
        internal const float ButtonSubtleContentHoverOffset = 2f;

        internal static Color HighlightActiveColor { get; } = Verse.Widgets.HighlightStrongBgColor.ToTransparent(0.5f);
    }

    internal static class MainTabWindow
    {
        internal const float ToolbarWidth = 40f;
        internal const float IconPadding = 5f;

        internal static Color BorderColor { get; } = new(1f, 1f, 1f, 0.4f);
    }

    public static class Text
    {
        public const float LineHeight = Verse.Text.SmallFontHeight;

        public static Color ColorHighlight { get; } = new(1f, 0.98f, 0.62f);

        public static Color ColorSecondary { get; } = Color.grey;

        public static GUIStyle FontMedium => Verse.Text.fontStyles[1];
    }

    internal static class Table
    {
        internal const float RowHeight = Text.LineHeight + TableCell.PadTB * 2f;
        internal const float HeadersRowHeight = RowHeight;

        internal static Color ColumnSeparatorLineColor { get; } = new(1f, 1f, 1f, 0.05f);

        internal static Color FixedPartSeparatorLineColor => HeadersRowBGColor;

        internal static Color HeadersRowBGColor { get; } = GenColor.FromBytes(56, 56, 60);
    }

    internal static class TableToolbar
    {
        internal const float Height = Text.LineHeight + TableCell.PadTB * 2f;
        internal const float Gap = Global.PadSm;
    }

    internal static class TableToolbarButton
    {
        internal const float IconWidth = Text.LineHeight;
        internal const float PadLR = Global.Pad;
        internal const float PadTB = TableCell.PadTB;

        internal static GUIStyle LabelStyle { get; } = new(_baseStyle)
        {
            alignment = TextAnchor.MiddleLeft,
        };
    }

    public static class TableCell
    {
        public const float ContentSpacing = PadLR / 2f;
        public const float PadLR = _PadLR;
        public const float PadTB = _PadTB;
        public const float PadHor = PadLR * 2f;
        public const float PadVer = PadTB * 2f;
        private const int _PadLR = 16;
        private const int _PadTB = 4;

        static TableCell()
        {
            RectOffset padding = new(_PadLR, _PadLR, _PadTB, _PadTB);

            StringNoPad = new GUIStyle(_baseStyle);
            StringNoPad.alignment = TextAnchor.LowerLeft;
            String = new GUIStyle(StringNoPad);
            String.padding = padding;

            NumberNoPad = new GUIStyle(_baseStyle);
            NumberNoPad.alignment = TextAnchor.LowerRight;
            Number = new GUIStyle(NumberNoPad);
            Number.padding = padding;

            BooleanNoPad = new GUIStyle(_baseStyle);
            BooleanNoPad.alignment = TextAnchor.LowerCenter;
            Boolean = new GUIStyle(BooleanNoPad);
            Boolean.padding = padding;
        }

        public static GUIStyle String { get; }

        public static GUIStyle StringNoPad { get; }

        public static GUIStyle Number { get; }

        public static GUIStyle NumberNoPad { get; }

        public static GUIStyle Boolean { get; }

        public static GUIStyle BooleanNoPad { get; }
    }
}
