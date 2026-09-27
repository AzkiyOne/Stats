using System.Collections.Generic;
using RimWorld;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class PlantDefTable : Tab
{
    public PlantDefTable(TableDef def) : base(def)
    {
        List<PlantDefTableRecord> records = new(250);

        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            PlantProperties? plantProperties = thingDef.plant;

            if (thingDef.IsPlant && plantProperties is { isStump: false })
            {
                PlantDefTableRecord record = new(thingDef, plantProperties);
                records.Add(record);
            }
        }

        Widget = new Table<PlantDefTableRecord>(def, records);
    }

    protected override TabBodyWidget Widget { get; }
}
