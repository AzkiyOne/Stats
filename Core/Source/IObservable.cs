using System;

namespace Stats;

public interface IObservable
{
    public event Action OnChange;
}
