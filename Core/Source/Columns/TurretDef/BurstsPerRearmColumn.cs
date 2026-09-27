using System.Collections.Generic;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.TurretDef;

public sealed class BurstsPerRearmColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public BurstsPerRearmColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool IsRefreshable => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        VerbProperties? turretGunDefPrimaryVerbProps = record.ThingDef.GetGunPrimaryVerbProps();

        if (turretGunDefPrimaryVerbProps != null)
        {
            CompProperties_Refuelable? refuelableCompProps = record.ThingDef.GetCompProperties<CompProperties_Refuelable>();

            if (refuelableCompProps is { fuelCapacity: > 0f })
            {
                float fuelPerBurst = turretGunDefPrimaryVerbProps.consumeFuelPerBurst;
                float fuelPerShot = turretGunDefPrimaryVerbProps.consumeFuelPerShot;

                if (fuelPerShot > 0f)
                {
                    fuelPerBurst = fuelPerShot * turretGunDefPrimaryVerbProps.burstShotCount;
                }

                if (fuelPerBurst > 0f)
                {
                    float fuelCapacity = refuelableCompProps.fuelCapacity;
                    float burstsPerRearm = fuelCapacity / fuelPerBurst;

                    return burstsPerRearm.ToDecimal();
                }
            }
        }

        return 0m;
    }
}
