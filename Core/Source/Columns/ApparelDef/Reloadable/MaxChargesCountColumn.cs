using System.Collections.Generic;
using RimWorld;
using Stats.TableRecords;

namespace Stats.Columns.ApparelDef.Reloadable;

public sealed class MaxChargesCountColumn<TRecord> : NumberColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public MaxChargesCountColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool AutoRefresh => false;

    protected override decimal GetValueFromRecord(TRecord record)
    {
        CompProperties_ApparelReloadable? reloadableCompProps = record.ThingDef.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProps != null)
        {
            return reloadableCompProps.maxCharges;
        }

        return 0m;
    }
}
