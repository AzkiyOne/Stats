namespace Stats.Widgets;

public sealed partial class Table<TRecord>
{
    // Note: Use row's index for stable sort (when two rows have the same cell values)?

    //private void SortRows()
    //{
    //    SortRows(_pinnedRows);
    //    SortRows(_unpinnedRows);

    //    DoSort = false;
    //}
    //private void SortRows(List<ObjectRow> rows)
    //{
    //    rows.Sort(CompareRows);
    //}
    //private int CompareRows(ObjectRow r1, ObjectRow r2)
    //{
    //    return r1.CompareToByColumn(r2, SortColumn) * SortDirection;
    //}
}
