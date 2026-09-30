using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.Extensions;
using UnityEngine;
using Verse;
using ButtonStyle = Stats.GUIStyles.TableToolbarButton;
using Style = Stats.GUIStyles.TableToolbar;

namespace Stats.Widgets;

public sealed partial class Table<TRecord>
{
    private sealed class Toolbar
    {
        private readonly Button _filtersButton;
        private readonly Button _columnsMenuButton;
        private readonly Button _columnPresetsButton;
        private readonly List<ColumnWidget> _columns;

        public Toolbar(List<ColumnWidget> columns)
        {
            _columns = columns;
            _filtersButton = new Button(Assets.TableFiltersTabIcon, "Filters");
            _columnsMenuButton = new Button(Assets.TableColumnsMenuIcon, "Columns");
            // TODO: Do we really need this feature if the plan is to save opened tables?
            _columnPresetsButton = new Button(Verse.TexButton.Paste, "Apply Preset");
        }

        public event Action? OnFiltersButtonClick;

        private FloatMenu ColumnsMenu => field ??= MakeColumnsMenu();

        public void Draw(Rect rect)
        {
            // Layout
            rect
                .CutLeft(out Rect filtersTabButtonRect, _filtersButton.Width)
                .CutLeft(Style.Gap)
                .CutLeft(out Rect columnsMenuButtonRect, _columnsMenuButton.Width)
                .CutLeft(Style.Gap)
                .CutLeft(out Rect columnPresetsButtonRect, _columnPresetsButton.Width)
                .CutRight(out Rect infoIconRect, rect.height);

            if (Event.current.type == EventType.Repaint)
            {
                rect.DrawBorderBottom(GUIStyles.MainTabWindow.BorderColor);
            }

            // Buttons
            bool filtersTabButtonWasClicked = _filtersButton.Draw(filtersTabButtonRect);
            bool columnsMenuButtonWasClicked = _columnsMenuButton.Draw(columnsMenuButtonRect);
            _columnPresetsButton.Draw(columnPresetsButtonRect);

            infoIconRect
                .ContractedBy(ButtonStyle.PadVer)
                .DrawTextureFitted(TexButton.Info)
                .Tip(_manual);

            // Events
            if (filtersTabButtonWasClicked)
            {
                OnFiltersButtonClick?.Invoke();
            }
            else if (columnsMenuButtonWasClicked)
            {
                ColumnsMenu.Open();
            }
        }

        private sealed class Button
        {
            private readonly Texture2D _icon;
            private readonly float _iconScale;
            private readonly string _label;

            public Button(Texture2D icon, string label, float iconScale = 1f)
            {
                _icon = icon;
                _iconScale = iconScale;
                _label = label;
                float labelWidth = label.CalcSize(ButtonStyle.LabelStyle).x;
                Width = ButtonStyle.PadHor * 2f + ButtonStyle.IconWidth + labelWidth;
            }

            public float Width { get; }

            public bool Draw(Rect rect)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    rect
                        .ContractedBy(ButtonStyle.PadHor, ButtonStyle.PadVer)
                        .CutLeft(out Rect iconRect, ButtonStyle.IconWidth)
                        .TakeRest(out Rect labelRect);

                    iconRect.DrawTextureFitted(_icon, _iconScale);
                    labelRect.DrawLabel(_label, ButtonStyle.LabelStyle);
                }

                return rect.DrawButtonGhostly();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private FloatMenu MakeColumnsMenu()
        {
            List<ColumnWidget> columns = _columns;
            int columnsCount = columns.Count;
            List<FloatMenuOption> columnsMenuOptions = new(columnsCount);

            for (int i = 0; i < columnsCount; i++)
            {
                ColumnWidget column = columns[i];
                ColumnsFloatMenuOption menuOption = new(column);

                columnsMenuOptions.Add(menuOption);
            }

            columnsMenuOptions.SortBy(option => option.Label);

            return new FloatMenu(columnsMenuOptions);
        }

        private sealed class ColumnsFloatMenuOption : FloatMenuOption
        {
            public ColumnsFloatMenuOption(ColumnWidget column)
                : base(column.Def.LabelCap, null, Verse.Widgets.CheckboxOnTex, Color.white)
            {
                ColumnDef columnDef = column.Def;
                Widget columnLabelWidget = columnDef.LabelWidget;

                tooltip = columnDef.description;
                action = () =>
                {
                    column.IsHidden = !column.IsHidden;
                };

                if (column.IsHidden)
                {
                    Unselect(column);
                }

                if (columnDef.title != null)
                {
                    extraPartOnGUI = rect =>
                    {
                        rect = rect.ContractedBy(0f, (rect.height - columnLabelWidget.Size.y) / 2f);
                        columnLabelWidget.Draw(rect);

                        return false;
                    };
                    extraPartWidth = columnLabelWidget.Size.x + GUIStyles.Global.PadSm;
                    extraPartRightJustified = true;
                }

                column.OnHide += Unselect;
                column.OnShow += Select;
            }

            private void Select(ColumnWidget column)
            {
                iconColor.a = 1f;
            }

            private void Unselect(ColumnWidget column)
            {
                iconColor.a = 0f;
            }

            public override bool DoGUI(Rect rect, bool colonistOrdering, FloatMenu floatMenu)
            {
                bool wordWrap = Verse.Text.WordWrap;
                Verse.Text.WordWrap = false;

                base.DoGUI(rect, colonistOrdering, floatMenu);

                Verse.Text.WordWrap = wordWrap;

                return false;
            }
        }
    }
}
