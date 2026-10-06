using System.Collections.Generic;
using RimWorld;
using Stats.TableRecords;

namespace Stats.Columns.ApparelDef.Reloadable;

public sealed class IsDestroyedOnEmptyColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public IsDestroyedOnEmptyColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    public override bool AutoRefresh => false;

    protected override bool GetValueFromRecord(TRecord record)
    {
        CompProperties_ApparelReloadable? reloadableCompProps = record.ThingDef.GetCompProperties<CompProperties_ApparelReloadable>();

        return reloadableCompProps?.destroyOnEmpty == true;
    }
}
