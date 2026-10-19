using RimWorld;
using Stats.TableRecords;
using System.Collections.Generic;

namespace Stats.Columns.ApparelDef.Reloadable;

public sealed class IsDestroyedOnEmptyColumn<TRecord> : BooleanColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public IsDestroyedOnEmptyColumn(ColumnDef def, List<TRecord> records, object _) : base(def, records)
    {
    }

    protected override bool GetValueFromRecord(TRecord record)
    {
        CompProperties_ApparelReloadable? reloadableCompProps = record.ThingDef.GetCompProperties<CompProperties_ApparelReloadable>();

        return reloadableCompProps?.destroyOnEmpty == true;
    }
}
