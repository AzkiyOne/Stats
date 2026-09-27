namespace Stats.Widgets;

public sealed partial class Table<TRecord>
{
    private void ToggleFiltersTab()
    {
    }
    //private void HandleFilterChange(Filter filter)
    //{
    //    if (filter.IsActive)
    //    {
    //        ActiveFilters.Add(filter);
    //    }
    //    else
    //    {
    //        ActiveFilters.Remove(filter);
    //    }

    //    DoFilter = true;
    //}
    //private void ApplyFilters()
    //{
    //    foreach (var row in _unpinnedRows)
    //    {
    //        var rowIsValid = true;

    //        if (ActiveFilters.Count > 0)
    //        {
    //            try
    //            {
    //                rowIsValid = MatchRowCells(row.Cells, ActiveFilters);
    //            }
    //            catch (Exception e)
    //            {
    //                Log.Error(e.Message);
    //            }
    //        }

    //        row.IsVisible = rowIsValid;
    //    }

    //    DoFilter = false;
    //    DoResize = true;
    //}
    //public override void ResetFilters()
    //{
    //    if (ActiveFilters.Count == 0)
    //        return;

    //    foreach (var filter in Filters)
    //    {
    //        if (filter.IsActive)
    //        {
    //            filter.Reset();
    //        }
    //    }
    //}
    //public override void ToggleFilterMode()
    //{
    //    FilterMode = FilterMode switch
    //    {
    //        TableFilterMode.AND => TableFilterMode.OR,
    //        TableFilterMode.OR => TableFilterMode.AND,
    //        _ => throw new NotSupportedException("Unsupported table filtering mode."),
    //    };
    //}

    //private readonly record struct Filter(Column Column, FilterWidget Widget)
    //{
    //    public bool IsActive => Widget.IsActive;
    //    public void Reset() => Widget.Reset();
    //}

    //private delegate bool RowCellsMatcher(Dictionary<Column, Cell> cells, HashSet<Filter> filters);
}
