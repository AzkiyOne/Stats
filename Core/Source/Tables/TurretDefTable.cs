using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using Stats.Widgets;
using Verse;

namespace Stats.Tables;

public sealed class TurretDefTable : Tab
{
    public TurretDefTable(TableDef def) : base(def)
    {
        List<TurretDefTableRecord> records = new(250);

        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            BuildingProperties? buildingProperties = thingDef.building;
            VerbProperties? primaryVerbProperties = buildingProperties?.turretGunDef?.Verbs.Primary();

            if (primaryVerbProperties != null
                && buildingProperties is { IsTurret: true, turretGunDef: Verse.ThingDef turretGunDef }
                && thingDef.IsBuildingObtainableByPlayer())
            {
                HashSet<Verse.ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (Verse.ThingDef stuffDef in stuffDefs)
                    {
                        TurretDefTableRecord record = new(thingDef, buildingProperties, turretGunDef, primaryVerbProperties, stuffDef);
                        records.Add(record);
                    }
                }
                else
                {
                    TurretDefTableRecord record = new(thingDef, buildingProperties, turretGunDef, primaryVerbProperties);
                    records.Add(record);
                }
            }
        }

        Widget = new Table<TurretDefTableRecord>(def, records);
    }

    protected override TabBodyWidget Widget { get; }
}
