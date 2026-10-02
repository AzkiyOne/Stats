using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Extensions;
using Stats.TableRecords;
using Verse;

namespace Stats.Columns.AnimalDef;

public sealed class ProductsColumn<TRecord> : ThingDefSetColumn<TRecord> where TRecord : IThingDefTableRecord
{
    public ProductsColumn(ColumnDef def, List<TRecord> records, IEnumerable<Verse.ThingDef> thingDefs) : base(def, records, GetProducts(thingDefs))
    {
    }

    public override bool IsRefreshable => false;

    protected override IReadOnlyCollection<Verse.ThingDef>? GetThingDefs(TRecord record)
    {
        return GetProducts(record.ThingDef);
    }

    private static HashSet<Verse.ThingDef> GetProducts(Verse.ThingDef thingDef)
    {
        HashSet<Verse.ThingDef> products = new(3);

        foreach (CompProperties compProperties in thingDef.comps)
        {
            if (compProperties is CompProperties_Milkable milkableCompProps)
            {
                products.Add(milkableCompProps.milkDef);
            }
            else if (compProperties is CompProperties_EggLayer eggLayerCompProps)
            {
                products.Add(eggLayerCompProps.GetAnyEggDef());
            }
            else if (compProperties is CompProperties_Shearable shearableCompProps)
            {
                products.Add(shearableCompProps.woolDef);
            }
        }

        return products;
    }

    private static IEnumerable<Verse.ThingDef> GetProducts(IEnumerable<Verse.ThingDef> thingDefs)
    {
        return thingDefs
            .SelectMany(GetProducts)
            .Distinct();
    }
}
