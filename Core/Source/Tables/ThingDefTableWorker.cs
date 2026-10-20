using Stats.Columns;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets;
using System;
using System.Collections.Generic;
using Verse;

namespace Stats.Tables;

public abstract class ThingDefTableWorker<TRecord> : TabWorker
{
    private readonly List<Column<TRecord>> _columns;
    private readonly int _recordsCount;

    protected ThingDefTableWorker(TableDef def)
    {
        List<TRecord> records = new(150);
        List<ThingDef> thingDefs = new(100);

        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (IsValidThingDef(thingDef))
            {
                thingDefs.Add(thingDef);

                HashSet<ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (ThingDef stuffDef in stuffDefs)
                    {
                        TRecord record = GetRecord(thingDef, stuffDef);

                        records.Add(record);
                    }
                }
                else
                {
                    TRecord record = GetRecord(thingDef, null);

                    records.Add(record);
                }
            }
        }

        records.TrimExcess();
        thingDefs.TrimExcess();

        _recordsCount = records.Count;

        List<Column<TRecord>> columns = new(def.columns.Count);

        foreach (TableColumnListItem columnListItem in def.columns)
        {
            ColumnDef columnDef = columnListItem.columnDef;

            try
            {
                Type columnType = columnDef.columnClass;

                if (columnType.IsGenericTypeDefinition)
                {
                    columnType = columnType.MakeGenericType(typeof(TRecord));
                }

                Column<TRecord> column = (Column<TRecord>)Activator.CreateInstance(columnType, columnDef, records, thingDefs);

                foreach (TRecord record in records)
                {
                    column.Add(record);
                }

                columns.Add(column);
            }
            catch (Exception e)
            {
                Log.Error($"Unable to initialize column \"{columnDef.defName}\" of table \"{def.defName}\": {e.Message}");
            }
        }

        _columns = columns;
    }

    public override IDrawable Widget => new Table(_columns, _recordsCount);

    protected abstract bool IsValidThingDef(ThingDef thingDef);

    protected abstract TRecord GetRecord(ThingDef thingDef, ThingDef? stuffDef);
}

public abstract class ThingDefTableWorker : ThingDefTableWorker<ThingDefTableRecord>
{
    protected ThingDefTableWorker(TableDef def) : base(def)
    {
    }

    protected override ThingDefTableRecord GetRecord(ThingDef thingDef, ThingDef? stuffDef)
    {
        return new ThingDefTableRecord(thingDef, stuffDef);
    }
}
