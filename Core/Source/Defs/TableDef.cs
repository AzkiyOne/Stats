using System.Collections.Generic;

namespace Stats;

public class TableDef : TabDef
{
#pragma warning disable CS8618
    public List<ColumnDef> columns;
#pragma warning restore CS8618
}
