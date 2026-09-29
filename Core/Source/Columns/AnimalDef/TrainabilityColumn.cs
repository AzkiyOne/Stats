using System.Collections.Generic;
using System.Linq;
using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class TrainabilityColumn<TRecord> : DefColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public TrainabilityColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetTrainabilityDefs(thingDefs))
    {
    }

    public override bool IsRefreshable => false;

    protected override Verse.Def? GetValueFromRecord(TRecord record)
    {
        return record.ThingDef.race?.trainability;
    }

    private static IEnumerable<Verse.Def?> GetTrainabilityDefs(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .Select<Verse.ThingDef, Verse.Def?>(thingDef => thingDef.race?.trainability)
            .Distinct();
    }
}
