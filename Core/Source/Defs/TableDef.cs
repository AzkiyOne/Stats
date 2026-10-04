using System.Collections.Generic;
using System.Xml;
using Verse;

namespace Stats;

public class TableDef : TabDef
{
#pragma warning disable CS8618
    public List<TableColumnListItem> columns;
#pragma warning restore CS8618
}

public class TableColumnListItem
{
    public ColumnDef columnDef;
    public bool isPrimary;

    public void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        string? isPrimaryAttrValueText = xmlRoot.Attributes["IsPrimary"]?.Value;

        if (isPrimaryAttrValueText != null)
        {
            isPrimary = ParseHelper.ParseBool(isPrimaryAttrValueText);
        }

        DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, nameof(columnDef), xmlRoot.InnerText);
    }
}
