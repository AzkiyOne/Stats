using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.TableRecords;

namespace Stats.Columns.PawnDef;

public sealed class WeaponsColumn<TRecord> : ThingDefSetColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public WeaponsColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetWeaponDefs(thingDefs))
    {
    }

    public override bool IsRefreshable => false;

    protected override IReadOnlyCollection<Verse.ThingDef>? GetThingDefs(TRecord record)
    {
        return record.ThingDef.GetPossibleWeapons();
    }

    private static IEnumerable<Verse.ThingDef> GetWeaponDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .SelectMany(thingDef => thingDef.GetPossibleWeapons() ?? [])
            .Distinct();
    }
}
