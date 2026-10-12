using System.Collections.Generic;

namespace Stats;

internal readonly struct ReadOnlyListSegment<T>
{
    private readonly List<T> _list;

    public ReadOnlyListSegment(List<T> list, int start, int length)
    {
        _list = list;
        Start = start;
        Length = length;
    }

    public int Start { get; }

    public int Length { get; }

    public T this[int index]
    {
        get => _list[index + Start];
    }
}
