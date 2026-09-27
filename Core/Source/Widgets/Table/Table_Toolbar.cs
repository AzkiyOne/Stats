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
        private readonly Table<TRecord> _parent;
        private readonly Button _filtersButton;
        private readonly Button _columnsMenuButton;
        private readonly Button _columnPresetsButton;
        private ColumnsFloatMenu ColumnsMenu => field ??= MakeColumnsMenu();

        public Toolbar(Table<TRecord> parent)
        {
            _parent = parent;
            _filtersButton = new Button(Assets.TableFiltersTabIcon, "Filters");
            _columnsMenuButton = new Button(Assets.TableColumnsMenuIcon, "Columns");
            // TODO: Do we really need this feature if plan to save opened tables?
            _columnPresetsButton = new Button(Verse.TexButton.Paste, "Apply Preset");
        }

        public void NotifyColumnAdded(ColumnWidget column)
        {
            ColumnsMenu.NotifyColumnAdded(column);
        }

        public void NotifyColumnRemoved(ColumnWidget column)
        {
            ColumnsMenu.NotifyColumnRemoved(column);
        }

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
                _parent.ToggleFiltersTab();
            }
            else if (columnsMenuButtonWasClicked)
            {
                ColumnsMenu.Open();
            }
        }

        private sealed class Button
        {
            public readonly float Width;

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
        private ColumnsFloatMenu MakeColumnsMenu()
        {
            List<ColumnDef> compatibleColumns = _parent._columnDefs;
            int compatibleColumnsCount = compatibleColumns.Count;
            List<FloatMenuOption> columnsMenuOptions = new(compatibleColumnsCount);
            for (int i = 0; i < compatibleColumnsCount; i++)
            {
                ColumnDef columnDef = compatibleColumns[i];
                ColumnsFloatMenuOption menuOption = new(columnDef, _parent);
                columnsMenuOptions.Add(menuOption);
            }
            columnsMenuOptions.SortBy(option => option.Label);

            return new ColumnsFloatMenu(columnsMenuOptions);
        }

        private sealed class ColumnsFloatMenu : FloatMenu
        {
            public ColumnsFloatMenu(List<FloatMenuOption> options) : base(options) { }

            public void NotifyColumnAdded(ColumnWidget column)
            {
                int optionsCount = options.Count;
                for (int i = 0; i < optionsCount; i++)
                {
                    ColumnsFloatMenuOption option = (ColumnsFloatMenuOption)options[i];

                    if (option.ColumnDef == column)
                    {
                        option.Select();
                        break;
                    }
                }
            }

            public void NotifyColumnRemoved(ColumnWidget column)
            {
                int optionsCount = options.Count;
                for (int i = 0; i < optionsCount; i++)
                {
                    ColumnsFloatMenuOption option = (ColumnsFloatMenuOption)options[i];
                    if (option.ColumnDef == column)
                    {
                        option.Unselect();
                        break;
                    }
                }
            }
        }

        private sealed class ColumnsFloatMenuOption : FloatMenuOption
        {
            public readonly ColumnDef ColumnDef;

            public ColumnsFloatMenuOption(ColumnDef columnDef, Table<TRecord> parent)
                : base(columnDef.LabelCap, null, Verse.Widgets.CheckboxOnTex, Color.white)
            {
                ColumnDef = columnDef;
                tooltip = columnDef.description;
                action = () =>
                {
                    if (parent._columns.Find(column => column == columnDef) != null)
                    {
                        parent.RemoveColumn(columnDef);
                    }
                    else
                    {
                        parent.AddColumn(columnDef);
                    }
                };

                if (parent._columns.Find(column => column == columnDef) == null)
                {
                    Unselect();
                }

                if (columnDef.title != null)
                {
                    extraPartOnGUI = rect =>
                    {
                        rect = rect.ContractedBy(0f, (rect.height - columnDef.LabelWidget.Size.y) / 2f);
                        columnDef.LabelWidget.Draw(rect);
                        return false;
                    };
                    extraPartWidth = columnDef.LabelWidget.Size.x + GUIStyles.Global.PadSm;
                    extraPartRightJustified = true;
                }
            }

            public void Select()
            {
                iconColor.a = 1f;
            }

            public void Unselect()
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
