using RimWorld;

namespace Stats.TableRecords;

public interface IBuildableDefTableRecord : IDefTableRecord
{
    Verse.BuildableDef BuildableDef { get; }

    StatRequest StatRequest { get; }
}
