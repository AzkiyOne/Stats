using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

// TODO: Not every power generator is a building (mods).
public sealed class PowerGeneratorDefTable : Tab
{
    public PowerGeneratorDefTable(TableDef def) : base(def)
    {
        List<BuildingDefTableRecord> records = new(250);

        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            BuildingProperties? buildingProperties = thingDef.building;

            if (buildingProperties != null
                && thingDef.IsBuildingObtainableByPlayer()
                && thingDef.GetCompProperties<CompProperties_Power>()?.PowerConsumption < 0f)
            {
                HashSet<Verse.ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (Verse.ThingDef stuffDef in stuffDefs)
                    {
                        BuildingDefTableRecord record = new(thingDef, buildingProperties, stuffDef);
                        records.Add(record);
                    }
                }
                else
                {
                    BuildingDefTableRecord record = new(thingDef, buildingProperties);
                    records.Add(record);
                }
            }
        }

        Widget = new Table<BuildingDefTableRecord>(def, records);
    }

    protected override TabBodyWidget Widget { get; }
}
