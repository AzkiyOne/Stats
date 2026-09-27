using System.Collections.Generic;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class MechanoidDefTable : Tab
{
    public MechanoidDefTable(TableDef def) : base(def)
    {
        List<PawnDefTableRecord> records = new(250);

        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            RaceProperties? raceProperties = thingDef.race;

            if (raceProperties != null && raceProperties.IsMechanoid && thingDef.IsCorpse == false)
            {
                PawnDefTableRecord tableRecord = new(thingDef, raceProperties);
                records.Add(tableRecord);
            }
        }

        Widget = new Table<PawnDefTableRecord>(def, records);
    }

    protected override TabBodyWidget Widget { get; }
}
