using System.Collections.Generic;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class AnimalDefTable : Tab
{
    public AnimalDefTable(TableDef def) : base(def)
    {
        List<PawnDefTableRecord> records = new(250);

        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            RaceProperties? raceProperties = thingDef.race;

            if (raceProperties != null && raceProperties.Animal && thingDef.IsCorpse == false)
            {
                PawnDefTableRecord record = new(thingDef, raceProperties);
                records.Add(record);
            }
        }

        Widget = new Table<PawnDefTableRecord>(def, records);
    }

    protected override TabBodyWidget Widget { get; }
}
