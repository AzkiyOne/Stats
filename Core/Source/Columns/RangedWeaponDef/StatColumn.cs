using RimWorld;
using Stats.Defs;
using Stats.Tables.RangedWeaponDef;

namespace Stats.Columns.RangedWeaponDef;

public sealed class StatColumn<TRecord> : BuildableDef.Columns.StatColumn<TRecord> where TRecord : IRangedWeaponDefTableRecord
{
    public StatColumn(StatColumnDef def) : base(def)
    {
    }

    protected override StatRequest GetStatRequest(TRecord record)
    {
        return record.RangedWeaponStatRequest;
    }
}
