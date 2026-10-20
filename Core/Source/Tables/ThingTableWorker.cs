using Stats.Columns;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets;
using System;
using System.Collections.Generic;
using Verse;

namespace Stats.Tables;

public abstract class ThingTableWorker<TRecord> : TabWorker, ITableDataSource
{
    private readonly List<Column<TRecord>> _columns;
    private readonly List<TRecord> _records;
    private readonly List<Thing> _things;

    protected ThingTableWorker(TableDef def)
    {
        // ThingDefs
        List<ThingDef> thingDefs = new(100);

        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (IsValidThingDef(thingDef))
            {
                thingDefs.Add(thingDef);
            }
        }

        thingDefs.TrimExcess();

        // Records & Things
        List<TRecord> records = new(150);
        List<Thing> things = new(records.Capacity);

        foreach (Thing thing in Find.Maps.GetSpawnedThings())
        {
            if (IsValidThing(thing))
            {
                TRecord record = GetRecord(thing);

                things.Add(thing);
                records.Add(record);
            }
        }

        records.TrimExcess();
        things.TrimExcess();

        _records = records;
        _things = things;

        // Columns
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

        // Events
        Events.ThingSpawned += thing =>
        {
            if (IsValidThing(thing))
            {
                TRecord record = GetRecord(thing);

                _records.Add(record);
                _things.Add(thing);

                foreach (Column<TRecord> column in columns)
                {
                    column.Add(record);
                }

                OnAdd?.Invoke();
            }
        };
        Events.ThingDespawned += thing =>
        {
            if (IsValidThing(thing))
            {
                int thingIndex = _things.IndexOf(thing);

                if (thingIndex != -1)
                {
                    int lastThingIndex = _things.Count - 1;

                    _things.Replace(thingIndex, lastThingIndex);
                    _records.Replace(thingIndex, lastThingIndex);

                    foreach (Column<TRecord> column in columns)
                    {
                        column.Replace(thingIndex, lastThingIndex);
                    }

                    OnReplace?.Invoke(thingIndex, lastThingIndex);
                }
            }
        };
    }

    public override IDrawable Widget => new Table(_columns, this);

    public event Action? OnAdd;

    public event Action<int, int>? OnReplace;

    public int RecordsCount => _records.Count;

    protected abstract bool IsValidThingDef(ThingDef thingDef);

    protected abstract bool IsValidThing(Thing thing);

    protected abstract TRecord GetRecord(Thing thing);
}

public abstract class ThingTableWorker : ThingTableWorker<ThingTableRecord>
{
    protected ThingTableWorker(TableDef def) : base(def)
    {
    }

    protected override ThingTableRecord GetRecord(Thing thing)
    {
        return new ThingTableRecord(thing);
    }
}
